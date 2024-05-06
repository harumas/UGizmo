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
        private readonly SharedGizmoBuffer<PrimitiveData> sharedGizmoBuffer;

        public PrimitiveGizmo()
        {
            sharedGizmoBuffer = SharedGizmoBuffer<PrimitiveData>.GetSharedBuffer();
        }
        
        public JobHandle Schedule()
        {
            var createJob = new CreatePrimitiveJob()
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