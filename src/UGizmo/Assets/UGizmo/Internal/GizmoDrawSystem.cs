using UGizmo.Internal.Extension.Gizmo;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
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

            drawers = activator.Drawers;
            preparingJobSchedulers = activator.PreparingJobSchedulers;
            jobSchedulers = activator.JobSchedulers;
        }

        public void ExecuteCreateJob()
        {
            //Run Continuous Gizmo
            foreach (var drawer in drawers.AsSpan())
            {
                drawer.EnqueueContinuousGizmo();
            }

            ExecutePrepareGizmoJob();
            ExecuteGizmoJob();

            if (!UGizmos.enabled) return;

            foreach (var drawer in drawers.AsSpan())
            {
                drawer.UploadGpuData();
            }
        }

        public void SetCommandBuffer(CommandBuffer commandBuffer)
        {
            if (!UGizmos.enabled) return;
            foreach (var drawer in drawers.AsSpan())
            {
                drawer.Draw(commandBuffer);
            }
        }

        public void DrawWithCamera(Camera camera)
        {
            if (!UGizmos.enabled) return;
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
            // Every reservation has to happen before any scheduling. Reserving grows the shared
            // job buffers, and once one of them exceeds its capacity it reallocates and frees the
            // old block - which would leave a pointer captured by an already-scheduled job
            // dangling, letting that job write into freed memory.
            foreach (var scheduler in preparingJobSchedulers.AsSpan())
            {
                // Always runs: this is what moves duration-based gizmos back into the queue, so it
                // decides whether the scheduler has data at all.
                scheduler.EnqueueContinuousGizmo();

                if (!scheduler.HasData)
                {
                    continue;
                }

                scheduler.ReserveBuffers();
            }

            foreach (var scheduler in preparingJobSchedulers.AsSpan())
            {
                if (!scheduler.HasData)
                {
                    continue;
                }

                preparingJobHandles.Add(scheduler.Schedule());
            }

            int length = preparingJobHandles.Length;

            if (length == 0)
            {
                return;
            }

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

            if (length == 0)
            {
                return;
            }

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