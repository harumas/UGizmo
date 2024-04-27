using System;
using Unity.Burst;
using Unity.Collections;

namespace UGizmo.Extension
{
    [BurstCompile]
    public unsafe class SharedGizmoBuffer<TJobData> : IDisposable where TJobData : unmanaged
    {
        public NativeList<TJobData> JobBuffer => jobBuffer;

        public NativeList<RenderData> RenderBuffer
        {
            get
            {
                renderBuffer.ResizeUninitialized(jobBuffer.Length);
                return renderBuffer;
            }
        }

        private static SharedGizmoBuffer<TJobData> instance;

        private NativeList<TJobData> jobBuffer = new NativeList<TJobData>(Allocator.Persistent);
        private NativeList<RenderData> renderBuffer = new NativeList<RenderData>(Allocator.Persistent);

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

        public (int start, int length) Reserve(int count, out TJobData* targetPtr)
        {
            int length = jobBuffer.Length;
            jobBuffer.ResizeUninitialized(length + count);
            targetPtr = jobBuffer.GetUnsafeList()->Ptr + length;
            return (length, count);
        }

        public void SetRenderData(NativeArray<int> handles, NativeArray<RenderData> destination, int count)
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