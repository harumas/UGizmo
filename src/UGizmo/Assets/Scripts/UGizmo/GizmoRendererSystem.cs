using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UGizmo
{
    public static class GizmoRendererSystem
    {
        private static ProfilingSampler profilingSampler;
        private static NoResizableList<IGizmoUpdater> updaters;
        private static NativeArray<JobHandle> jobHandles;

        [InitializeOnLoadMethod]
        public static void Init()
        {
            RenderPipelineManager.endFrameRendering += OnEndCameraRendering;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            profilingSampler = new ProfilingSampler("DrawUGizmos");
            jobHandles = new NativeArray<JobHandle>(64, Allocator.Persistent);

            GizmoInstanceActivator activator = new GizmoInstanceActivator();
            updaters = activator.CreateInstance();
        }

        private static void OnBeforeAssemblyReload()
        {
            foreach (IGizmoUpdater updater in updaters.AsSpan())
            {
                updater.Dispose();
            }

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
                var updaterSpan = updaters.AsSpan();

                int i = 0;
                foreach (var updater in updaterSpan)
                {
                    jobHandles[i++] = updater.CreateJobHandle();
                }

                JobHandle createDataJob = JobHandle.CombineDependencies(jobHandles.Slice(0, updaterSpan.Length));
                createDataJob.Complete();

                foreach (IGizmoUpdater updater in updaters.AsSpan())
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