using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UGizmo.Internal.Utility;
using Unity.Collections;
using UnityEngine;

namespace UGizmo.Internal
{
    internal unsafe class GizmoDrawBuffer<TJobData> : IDisposable where TJobData : unmanaged
    {
        public int Count { get; private set; } = 0;

        private const int InitialCapacity = 8192;

        private int* handleBuffer;
        private DrawData* drawBuffer;

        private GraphicsBuffer graphicsBuffer;
        private readonly SharedGizmoBuffer<TJobData> jobDataBuffer;
        private readonly MaterialPropertyBlock propertyBlock;
        private Action<IntPtr, int, int, int, int> setNativeData;

        public GizmoDrawBuffer()
        {
            handleBuffer = UnmanagedUtility.Malloc<int>(InitialCapacity, Allocator.Persistent);
            drawBuffer = UnmanagedUtility.Malloc<DrawData>(InitialCapacity, Allocator.Persistent);

            jobDataBuffer = SharedGizmoBuffer<TJobData>.GetSharedBuffer();
            propertyBlock = new MaterialPropertyBlock();

            AllocateGraphicsBuffer(InitialCapacity);
        }
        
        private void AllocateGraphicsBuffer(int capacity)
        {
            graphicsBuffer?.Dispose();
            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, capacity, Marshal.SizeOf<DrawData>());
            
            propertyBlock.SetBuffer(Shader.PropertyToID("_DrawBuffer"), graphicsBuffer);
            setNativeData = UnityInternalUtility.CreateInternalSetDataDelegate(graphicsBuffer);
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

            //Treats a range of indexes as handles.
            for (int i = 0; i < handle.length; i++)
            {
                handleBuffer[offset + i] = handle.start + i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TJobData* Reserve(int count)
        {
            EnsureCapacity(count);

            int offset = Count;
            Count += count;

            (int start, int length) handle = jobDataBuffer.Reserve(count, out TJobData* ptr);

            //Treats a range of indexes as handles.
            for (int i = 0; i < handle.length; i++)
            {
                handleBuffer[offset + i] = handle.start + i;
            }

            return ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UploadGpuData()
        {
            jobDataBuffer.SetRenderData(handleBuffer, drawBuffer, Count);
            setNativeData((IntPtr)drawBuffer, 0, 0, Count, sizeof(DrawData));
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
                UnmanagedUtility.ResizePointer(ref drawBuffer, count, newCapacity, Allocator.Persistent);
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
            UnmanagedUtility.Free(ref drawBuffer, Allocator.Persistent);
            graphicsBuffer.Dispose();
        }
    }
}