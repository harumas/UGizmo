using UGizmo.Extension.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UGizmo.Extension
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