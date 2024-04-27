using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension.Gizmo
{
    public unsafe class WireLineGizmo : IGizmoJobScheduler
    {
        private readonly SharedGizmoBuffer<LineData> gizmoBuffer;

        public WireLineGizmo()
        {
            gizmoBuffer = SharedGizmoBuffer<LineData>.GetSharedBuffer();
        }
        
        public JobHandle Schedule()
        {
            var createJob = new CreateWireLineJob()
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