using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UGizmo.Internal
{
    [BurstCompile]
    internal unsafe class SharedGizmoBuffer<TJobData> : IDisposable where TJobData : unmanaged
    {
        public UnsafeList<TJobData> JobBuffer => jobBuffer;

        public UnsafeList<RenderData> RenderBuffer
        {
            get
            {
                renderBuffer.Resize(jobBuffer.Length);
                return renderBuffer;
            }
        }

        private static SharedGizmoBuffer<TJobData> instance;

        private const int InitialCapacity = 4192;
        private UnsafeList<TJobData> jobBuffer = new UnsafeList<TJobData>(InitialCapacity, Allocator.Persistent);
        private UnsafeList<RenderData> renderBuffer = new UnsafeList<RenderData>(InitialCapacity, Allocator.Persistent);

        public static SharedGizmoBuffer<TJobData> GetSharedBuffer()
        {
            return instance ??= new SharedGizmoBuffer<TJobData>();
        }

        public int Add(in TJobData jobData)
        {
            int handle = jobBuffer.Length;
            jobBuffer.Add(jobData);
            return handle;
        }

        public (int start, int length) AddRange(TJobData* jobData, int count)
        {
            int length = jobBuffer.Length;
            jobBuffer.AddRange(jobData, count);
            return (length, count);
        }

        public (int start, int length) Reserve(int count, out TJobData* targetPtr)
        {
            int length = jobBuffer.Length;
            jobBuffer.Resize(length + count);
            targetPtr = jobBuffer.Ptr + length;
            return (length, count);
        }

        [BurstCompile]
        public void SetRenderData(int* handles, RenderData* destination, int count)
        {
            for (int i = 0; i < count; i++)
            {
                destination[i] = renderBuffer[handles[i]];
            }
        }

        public void Clear()
        {
            jobBuffer.Clear();
            renderBuffer.Clear();
        }

        public void Dispose()
        {
            jobBuffer.Dispose();
            renderBuffer.Dispose();
        }
    }
}