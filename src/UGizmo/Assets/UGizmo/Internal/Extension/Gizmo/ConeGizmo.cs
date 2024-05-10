using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension.Gizmo
{
    public unsafe class ConeGizmo : IGizmoJobScheduler
    {
        private readonly SharedGizmoBuffer<ConeData> sharedGizmoBuffer;

        public ConeGizmo()
        {
            sharedGizmoBuffer = SharedGizmoBuffer<ConeData>.GetSharedBuffer();
        }

        public JobHandle Schedule()
        {
            var createJob = new CreateConeJob()
            {
                GizmoDataPtr = sharedGizmoBuffer.JobBuffer.Ptr,
                Result = sharedGizmoBuffer.DrawBuffer.Ptr
            };

            return createJob.Schedule(sharedGizmoBuffer.JobBuffer.Length, 16);
        }

        public void Clear()
        {
            sharedGizmoBuffer.Clear();
        }

        public void Dispose()
        {
            sharedGizmoBuffer?.Dispose();
        }
    }
}