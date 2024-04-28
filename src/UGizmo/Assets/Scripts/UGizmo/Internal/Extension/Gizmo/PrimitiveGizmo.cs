using System;
using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension.Gizmo
{
    public interface IGizmoJobScheduler : IDisposable
    {
         JobHandle Schedule();
         void Clear();
    }

    public unsafe class PrimitiveGizmo : IGizmoJobScheduler
    {
        private readonly SharedGizmoBuffer<PrimitiveData> gizmoBuffer;

        public PrimitiveGizmo()
        {
            gizmoBuffer = SharedGizmoBuffer<PrimitiveData>.GetSharedBuffer();
        }
        
        public JobHandle Schedule()
        {
            var createJob = new CreatePrimitiveJob()
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