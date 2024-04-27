using System;
using UGizmo.Extension.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UGizmo.Extension
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