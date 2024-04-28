using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension.Gizmo
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
                GizmoDataPtr = gizmoBuffer.JobBuffer.Ptr,
                Result = gizmoBuffer.RenderBuffer.Ptr
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