using UGizmo.Extension.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public unsafe class ConeGizmo : IGizmoJobScheduler
    {
        private readonly SharedGizmoBuffer<ConeData> gizmoBuffer;

        public ConeGizmo()
        {
            gizmoBuffer = SharedGizmoBuffer<ConeData>.GetSharedBuffer();
        }

        public JobHandle Schedule()
        {
            var createJob = new CreateConeJob()
            {
                GizmoDataPtr = gizmoBuffer.JobBuffer.GetUnsafeReadOnlyPtr(),
                Result = gizmoBuffer.RenderBuffer.GetUnsafePtr()
            };

            return createJob.Schedule(gizmoBuffer.JobBuffer.Length, 16);
        }

        public void Clear()
        {
            gizmoBuffer.Clear();
        }

        public void Dispose()
        {
            gizmoBuffer?.Dispose();
        }
    }
}