using System.Collections.Generic;
using UGizmo.Internal.Extension.Gizmo;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UGizmo.Internal
{
    internal static unsafe class GizmoRendererSystem
    {
        private static ProfilingSampler profilingSampler;
        private static NoResizableList<IGizmoRenderer> renderers;
        private static NoResizableList<IPreparingJobScheduler> preparingJobSchedulers;
        private static NoResizableList<IGizmoJobScheduler> jobSchedulers;
        private static GizmoInstanceActivator activator;
        private static UnsafeList<JobHandle> preparingJobHandles;
        private static UnsafeList<JobHandle> jobHandles;

        private const int InitialCapacity = 64;

        [InitializeOnLoadMethod]
        public static void Init()
        {
            RenderPipelineManager.endContextRendering += OnEndCameraRendering;
            DisposeEvent.instance.Dispose += OnDispose;

            profilingSampler = new ProfilingSampler("DrawUGizmos");
            preparingJobHandles = new UnsafeList<JobHandle>(InitialCapacity, Allocator.Persistent);
            jobHandles = new UnsafeList<JobHandle>(InitialCapacity, Allocator.Persistent);

            activator = new GizmoInstanceActivator();
            activator.Activate();

            renderers = activator.Updaters;
            preparingJobSchedulers = activator.PreparingJobSchedulers;
            jobSchedulers = activator.JobSchedulers;
        }

        private static void OnDispose()
        {
            activator.Dispose();
            preparingJobHandles.Dispose();
            jobHandles.Dispose();
        }

        private static void OnEndCameraRendering(ScriptableRenderContext context, List<Camera> _)
        {
            if (!Handles.ShouldRenderGizmos())
            {
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get();


            using (new ProfilingScope(cmd, profilingSampler))
            {
                ExecutePrepareGizmoJob();
                ExecuteGizmoJob();

                foreach (var updater in renderers.AsSpan())
                {
                    updater.Render(cmd);
                }

                foreach (var scheduler in jobSchedulers.AsSpan())
                {
                    scheduler.Clear();
                }
            }

            context.ExecuteCommandBuffer(cmd);
            context.Submit();

            CommandBufferPool.Release(cmd);
        }

        private static void ExecutePrepareGizmoJob()
        {
            foreach (var scheduler in preparingJobSchedulers.AsSpan())
            {
                preparingJobHandles.Add(scheduler.Schedule());
                scheduler.Clear();
            }

            int length = preparingJobHandles.Length;

            JobHandle preparingJobHandle = JobHandleUnsafeUtility.CombineDependencies(preparingJobHandles.Ptr, length);
            preparingJobHandle.Complete();
            preparingJobHandles.Clear();
        }

        private static void ExecuteGizmoJob()
        {
            foreach (var scheduler in jobSchedulers.AsSpan())
            {
                jobHandles.Add(scheduler.Schedule());
            }

            int length = jobHandles.Length;

            JobHandle jobHandle = JobHandleUnsafeUtility.CombineDependencies(jobHandles.Ptr, length);
            jobHandle.Complete();
            jobHandles.Clear();
        }
    }
}