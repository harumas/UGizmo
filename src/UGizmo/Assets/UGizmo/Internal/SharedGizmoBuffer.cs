using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace UGizmo.Internal
{
    [BurstCompile]
    internal unsafe class SharedGizmoBuffer<TJobData> : IDisposable where TJobData : unmanaged
    {
        public ref UnsafeList<TJobData> JobBuffer => ref backJobBuffer.Ref;

        public ref UnsafeList<RenderData> RenderBuffer
        {
            get
            {
                renderBuffer.Resize(backJobBuffer.Ref.Length);
                return ref renderBuffer;
            }
        }

        private static SharedGizmoBuffer<TJobData> instance;

        public static SharedGizmoBuffer<TJobData> GetSharedBuffer()
        {
            return instance ??= new SharedGizmoBuffer<TJobData>();
        }

        private const int InitialCapacity = 4192;
        private UnsafeListReference<TJobData> frontJobBuffer = new UnsafeListReference<TJobData>(InitialCapacity, Allocator.Persistent);
        private UnsafeListReference<TJobData> backJobBuffer = new UnsafeListReference<TJobData>(InitialCapacity, Allocator.Persistent);
        private UnsafeList<RenderData> renderBuffer = new UnsafeList<RenderData>(InitialCapacity, Allocator.Persistent);

        public int Add(in TJobData jobData)
        {
            int handle = backJobBuffer.Ref.Length;
            backJobBuffer.Ref.Add(jobData);
            return handle;
        }

        public (int start, int length) AddRange(TJobData* jobData, int count)
        {
            int length = backJobBuffer.Ref.Length;
            backJobBuffer.Ref.AddRange(jobData, count);
            return (length, count);
        }

        public (int start, int length) Reserve(int count, out TJobData* targetPtr)
        {
            int length = backJobBuffer.Ref.Length;
            backJobBuffer.Ref.Resize(length + count);
            targetPtr = backJobBuffer.Ref.Ptr + length;
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

        public void Swap()
        {
            (frontJobBuffer, backJobBuffer) = (backJobBuffer, frontJobBuffer);
            backJobBuffer.Ref.Clear();
        }

        public void Clear()
        {
            backJobBuffer.Ref.Clear();
            renderBuffer.Clear();
        }

        public void Dispose()
        {
            frontJobBuffer.Dispose();
            backJobBuffer.Dispose();
            renderBuffer.Dispose();
        }
    }
}