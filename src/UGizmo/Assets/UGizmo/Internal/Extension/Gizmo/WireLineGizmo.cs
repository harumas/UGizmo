using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension.Gizmo
{
    public unsafe class WireLineGizmo : IGizmoJobScheduler
    {
        private readonly SharedGizmoBuffer<LineData> sharedGizmoBuffer;

        public WireLineGizmo()
        {
            sharedGizmoBuffer = SharedGizmoBuffer<LineData>.GetSharedBuffer();
        }
        
        public JobHandle Schedule()
        {
            var createJob = new CreateWireLineJob()
            {
                GizmoDataPtr = sharedGizmoBuffer.JobBuffer.Ptr,
                Result = sharedGizmoBuffer.RenderBuffer.Ptr
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