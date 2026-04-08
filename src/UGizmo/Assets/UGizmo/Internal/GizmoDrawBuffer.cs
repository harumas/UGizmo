using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UGizmo.Internal.Utility;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace UGizmo.Internal
{
    [BurstCompile]
    internal unsafe class GizmoDrawBuffer<TJobData> : IDisposable where TJobData : unmanaged
    {
        private static readonly int DrawBufferPropertyId = Shader.PropertyToID("_DrawBuffer");
        private static readonly int DrawDataSize = Marshal.SizeOf<DrawData>();

        public int Count { get; private set; } = 0;

        private const int InitialCapacity = 8192;

        private int* handleBuffer;

        private GraphicsBuffer graphicsBuffer;
        private readonly SharedGizmoBuffer<TJobData> jobDataBuffer;
        private readonly MaterialPropertyBlock propertyBlock;

        public GizmoDrawBuffer()
        {
            handleBuffer = UnmanagedUtility.Malloc<int>(InitialCapacity, Allocator.Persistent);

            jobDataBuffer = SharedGizmoBuffer<TJobData>.GetSharedBuffer();
            propertyBlock = new MaterialPropertyBlock();

            AllocateGraphicsBuffer(InitialCapacity);
        }
        
        private void AllocateGraphicsBuffer(int capacity)
        {
            graphicsBuffer?.Dispose();
            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, GraphicsBuffer.UsageFlags.LockBufferForWrite, capacity, DrawDataSize);
            
            propertyBlock.SetBuffer(DrawBufferPropertyId, graphicsBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TJobData data)
        {
            EnsureCapacity(1);

            handleBuffer[Count++] = jobDataBuffer.Add(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(TJobData* data, int count)
        {
            EnsureCapacity(count);

            int offset = Count;
            Count += count;
            (int start, int length) handle = jobDataBuffer.AddRange(data, count);

            FillSequentialHandles(handleBuffer + offset, handle.start, handle.length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TJobData* Reserve(int count)
        {
            EnsureCapacity(count);

            int offset = Count;
            Count += count;

            (int start, int length) handle = jobDataBuffer.Reserve(count, out TJobData* ptr);

            FillSequentialHandles(handleBuffer + offset, handle.start, handle.length);

            return ptr;
        }

        [BurstCompile]
        private static void FillSequentialHandles(int* buffer, int start, int count)
        {
            for (int i = 0; i < count; i++)
            {
                buffer[i] = start + i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UploadGpuData()
        {
            if (Count == 0) return;

            NativeArray<DrawData> gpuBuffer = graphicsBuffer.LockBufferForWrite<DrawData>(0, Count);
            DrawData* gpuPtr = (DrawData*)gpuBuffer.GetUnsafePtr();

            jobDataBuffer.SetRenderData(handleBuffer, gpuPtr, Count);

            graphicsBuffer.UnlockBufferAfterWrite<DrawData>(Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MaterialPropertyBlock GetPropertyBlock()
        {
            return propertyBlock;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int offset)
        {
            if (Count + offset > graphicsBuffer.count)
            {
                int count = graphicsBuffer.count;
                int newCapacity = Math.Max(Count + offset, count * 2);

                AllocateGraphicsBuffer(newCapacity);

                UnmanagedUtility.ResizePointer(ref handleBuffer, count, newCapacity, Allocator.Persistent);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Count = 0;
        }

        public void Dispose()
        {
            UnmanagedUtility.Free(ref handleBuffer, Allocator.Persistent);
            graphicsBuffer.Dispose();
        }
    }
}