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
            int length = sharedGizmoBuffer.JobBuffer.Length;

            // Nothing queued this frame. Skipping avoids scheduling an empty parallel job, and also
            // avoids the resize that reading DrawBuffer would trigger.
            if (length == 0)
            {
                return default;
            }

            var createJob = new CreatePrimitiveJob()
            {
                GizmoDataPtr = sharedGizmoBuffer.JobBuffer.Ptr,
                Result = sharedGizmoBuffer.DrawBuffer.Ptr
            };

            return createJob.Schedule(length, 16);
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