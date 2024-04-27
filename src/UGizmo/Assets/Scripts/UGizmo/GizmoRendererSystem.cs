using System;
using UGizmo.Extension;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UGizmo
{
    public class DisposeEvent : ScriptableSingleton<DisposeEvent>
    {
        public event Action Dispose;

        private void OnDisable()
        {
            Dispose?.Invoke();
            Dispose = null;
        }
    }

    public static class GizmoRendererSystem
    {
        private static ProfilingSampler profilingSampler;
        private static NoResizableList<IGizmoRenderer> renderers;
        private static NoResizableList<IPreparingJobScheduler> preparingJobSchedulers;
        private static NoResizableList<IGizmoJobScheduler> jobSchedulers;
        private static GizmoInstanceActivator activator;
        private static NativeArray<JobHandle> preparingJobHandles;
        private static NativeArray<JobHandle> jobHandles;

        [InitializeOnLoadMethod]
        public static void Init()
        {
            RenderPipelineManager.endFrameRendering += OnEndCameraRendering;
            DisposeEvent.instance.Dispose += OnDispose;
            profilingSampler = new ProfilingSampler("DrawUGizmos");
            preparingJobHandles = new NativeArray<JobHandle>(64, Allocator.Persistent);
            jobHandles = new NativeArray<JobHandle>(64, Allocator.Persistent);

            activator = new GizmoInstanceActivator();
            activator.Activate();

            renderers = activator.Updaters;
            preparingJobSchedulers = activator.PreparingJobSchedulers;
            jobSchedulers = activator.JobSchedulers;
        }

        private static void OnDispose()
        {
            activator.Dispose();
            jobHandles.Dispose();
        }

        private static void OnEndCameraRendering(ScriptableRenderContext context, Camera[] cameras)
        {
            if (!Handles.ShouldRenderGizmos())
            {
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, profilingSampler))
            {
                int i = 0;
                foreach (var scheduler in preparingJobSchedulers.AsSpan())
                {
                    preparingJobHandles[i++] = scheduler.Schedule();
                    scheduler.Clear();
                }

                JobHandle preparingJobHandle = JobHandle.CombineDependencies(jobHandles.Slice(0, preparingJobSchedulers.Count));
                preparingJobHandle.Complete();

                i = 0;
                foreach (var scheduler in jobSchedulers.AsSpan())
                {
                    jobHandles[i++] = scheduler.Schedule();
                }

                JobHandle createDataJob = JobHandle.CombineDependencies(jobHandles.Slice(0, jobSchedulers.Count));
                createDataJob.Complete();

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
    }
}