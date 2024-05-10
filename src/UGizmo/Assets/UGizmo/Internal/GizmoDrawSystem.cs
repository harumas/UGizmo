using UGizmo.Internal.Extension.Gizmo;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;

#else
using UnityEngine;
#endif


namespace UGizmo.Internal
{
    internal unsafe class GizmoDrawSystem
    {
        private NoResizableList<IGizmoDrawer> drawers;
        private NoResizableList<IPreparingJobScheduler> preparingJobSchedulers;
        private NoResizableList<IGizmoJobScheduler> jobSchedulers;
        private GizmoInstanceActivator activator;
        private UnsafeList<JobHandle> preparingJobHandles;
        private UnsafeList<JobHandle> jobHandles;

        private const int InitialCapacity = 64;

        public void Initialize()
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

            drawers = activator.Updaters;
            preparingJobSchedulers = activator.PreparingJobSchedulers;
            jobSchedulers = activator.JobSchedulers;
        }

        public void ExecuteCreateJob()
        {
            foreach (var drawer in drawers.AsSpan())
            {
                drawer.EnqueueContinuousGizmo();
            }

            ExecutePrepareGizmoJob();
            ExecuteGizmoJob();

            foreach (var drawer in drawers.AsSpan())
            {
                drawer.UploadGpuData();
            }
        }

        public void SetCommandBuffer(CommandBuffer commandBuffer)
        {
            foreach (var drawer in drawers.AsSpan())
            {
                drawer.Draw(commandBuffer);
            }
        }

        public void DrawWithCamera(Camera camera)
        {
            foreach (IGizmoDrawer drawer in drawers.AsSpan())
            {
                drawer.DrawWithCamera(camera);
            }
        }

        public void ClearContinuousGizmo()
        {
            foreach (IGizmoDrawer drawer in drawers.AsSpan())
            {
                drawer.ClearContinuousGizmo();
            }

            foreach (IPreparingJobScheduler scheduler in preparingJobSchedulers.AsSpan())
            {
                scheduler.ClearContinuousGizmo();
            }
        }

        public void ClearScheduler()
        {
            foreach (IGizmoJobScheduler scheduler in jobSchedulers.AsSpan())
            {
                scheduler.Clear();
            }

            foreach (IPreparingJobScheduler scheduler in preparingJobSchedulers.AsSpan())
            {
                scheduler.Clear();
            }
        }

        private void ExecutePrepareGizmoJob()
        {
            foreach (var scheduler in preparingJobSchedulers.AsSpan())
            {
                scheduler.EnqueueContinuousGizmo();
                preparingJobHandles.Add(scheduler.Schedule());
            }

            int length = preparingJobHandles.Length;

            JobHandle preparingJobHandle = JobHandleUnsafeUtility.CombineDependencies(preparingJobHandles.Ptr, length);
            preparingJobHandle.Complete();
            preparingJobHandles.Clear();
        }

        private void ExecuteGizmoJob()
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

        private void OnDispose()
        {
            activator.Dispose();
            preparingJobHandles.Dispose();
            jobHandles.Dispose();
        }
    }
}