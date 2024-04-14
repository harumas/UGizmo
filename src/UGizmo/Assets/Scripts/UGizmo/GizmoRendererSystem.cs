using System;
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
        private static NoResizableList<IGizmoRenderer> updaters;
        private static GizmoInstanceActivator activator;
        private static NoResizableList<IPreparingJobScheduler> schedulers;
        private static NativeArray<JobHandle> jobHandles;

        [InitializeOnLoadMethod]
        public static void Init()
        {
            RenderPipelineManager.endFrameRendering += OnEndCameraRendering;
            DisposeEvent.instance.Dispose += OnDispose;
            profilingSampler = new ProfilingSampler("DrawUGizmos");
            jobHandles = new NativeArray<JobHandle>(64, Allocator.Persistent);

            activator = new GizmoInstanceActivator();
            activator.Activate();

            updaters = activator.Updaters;
            schedulers = activator.Schedulers;
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
                foreach (var scheduler in schedulers.AsSpan())
                {
                    scheduler.Schedule();
                    scheduler.Clear();
                }
                
                var updaterSpan = updaters.AsSpan();

                int i = 0;
                foreach (var updater in updaterSpan)
                {
                    jobHandles[i++] = updater.CreateJobHandle();
                }

                JobHandle createDataJob = JobHandle.CombineDependencies(jobHandles.Slice(0, updaterSpan.Length));
                createDataJob.Complete();

                foreach (var updater in updaterSpan)
                {
                    updater.Render(cmd);
                }
            }

            context.ExecuteCommandBuffer(cmd);
            context.Submit();

            CommandBufferPool.Release(cmd);
        }
    }
}