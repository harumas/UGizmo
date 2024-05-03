using UGizmo.Internal.Extension.Gizmo;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;

#else
using UnityEngine;
#endif


namespace UGizmo.Internal
{
    internal static unsafe class GizmoRendererSystem
    {
        private static NoResizableList<IGizmoRenderer> renderers;
        private static NoResizableList<IPreparingJobScheduler> preparingJobSchedulers;
        private static NoResizableList<IGizmoJobScheduler> jobSchedulers;
        private static GizmoInstanceActivator activator;
        private static UnsafeList<JobHandle> preparingJobHandles;
        private static UnsafeList<JobHandle> jobHandles;

        private const int InitialCapacity = 64;
        private static bool isInitialized;

        public static void Initialize()
        {
#if UNITY_EDITOR
            AssemblyReloadEvents.beforeAssemblyReload += OnDispose;
#else
            Application.quitting += OnDispose;
#endif

            preparingJobHandles = new UnsafeList<JobHandle>(InitialCapacity, Allocator.Persistent);
            jobHandles = new UnsafeList<JobHandle>(InitialCapacity, Allocator.Persistent);

            activator = new GizmoInstanceActivator();
            activator.Activate();

            renderers = activator.Updaters;
            preparingJobSchedulers = activator.PreparingJobSchedulers;
            jobSchedulers = activator.JobSchedulers;

            isInitialized = true;
        }

        public static void SetupCommandBuffer(CommandBuffer commandBuffer)
        {
            if (!isInitialized
#if UNITY_EDITOR
                || !Handles.ShouldRenderGizmos()
#endif
               )
            {
                return;
            }

            ExecutePrepareGizmoJob();
            ExecuteGizmoJob();

            foreach (var updater in renderers.AsSpan())
            {
                updater.Render(commandBuffer);
            }

            foreach (var scheduler in jobSchedulers.AsSpan())
            {
                scheduler.Clear();
            }
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

        private static void OnDispose()
        {
            activator.Dispose();
            preparingJobHandles.Dispose();
            jobHandles.Dispose();
        }
    }
}