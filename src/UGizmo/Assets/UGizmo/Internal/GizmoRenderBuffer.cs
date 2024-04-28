using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace UGizmo.Internal
{
    internal unsafe class GizmoRenderBuffer<TJobData> : IDisposable where TJobData : unmanaged
    {
        public int Count { get; private set; } = 0;

        private const int InitialCapacity = 8192;

        private int* handleBuffer;
        private RenderData* renderBuffer;

        private GraphicsBuffer graphicsBuffer;
        private readonly SharedGizmoBuffer<TJobData> gizmoBuffer;
        private readonly MaterialPropertyBlock propertyBlock;
        private Action<IntPtr, int, int, int, int> setNativeData;

        private static readonly MethodInfo setNativeDataMethod;
        private static readonly int renderBufferProperty = Shader.PropertyToID("_RenderBuffer");

        static GizmoRenderBuffer()
        {
            setNativeDataMethod = typeof(GraphicsBuffer).GetRuntimeMethods().First(method => method.Name == "InternalSetNativeData");
        }

        public GizmoRenderBuffer()
        {
            handleBuffer = UnmanagedUtility.Malloc<int>(InitialCapacity, Allocator.Persistent);
            renderBuffer = UnmanagedUtility.Malloc<RenderData>(InitialCapacity, Allocator.Persistent);

            gizmoBuffer = SharedGizmoBuffer<TJobData>.GetSharedBuffer();
            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, InitialCapacity, Marshal.SizeOf<RenderData>());
            propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetBuffer(renderBufferProperty, graphicsBuffer);

            setNativeData = CreateInternalSetDataDelegate();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TJobData data)
        {
            CheckCapacity();

            handleBuffer[Count++] = gizmoBuffer.Add(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(TJobData* data, int count)
        {
            CheckCapacity();

            int offset = Count;
            Count += count;
            (int start, int length) handle = gizmoBuffer.AddRange(data, count);

            for (int i = 0; i < handle.length; i++)
            {
                handleBuffer[offset + i] = handle.start + i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TJobData* Reserve(int count)
        {
            CheckCapacity();

            int offset = Count;
            Count += count;

            (int start, int length) handle = gizmoBuffer.Reserve(count, out TJobData* ptr);

            for (int i = 0; i < handle.length; i++)
            {
                handleBuffer[offset + i] = handle.start + i;
            }

            return ptr;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UploadGpuData()
        {
            gizmoBuffer.SetRenderData(handleBuffer, renderBuffer, Count);
            setNativeData((IntPtr)renderBuffer, 0, 0, Count, sizeof(RenderData));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MaterialPropertyBlock GetPropertyBlock()
        {
            return propertyBlock;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckCapacity()
        {
            if (Count >= graphicsBuffer.count)
            {
                int count = graphicsBuffer.count;
                graphicsBuffer?.Dispose();
                graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count * 2, Marshal.SizeOf<RenderData>());
                propertyBlock.SetBuffer(renderBufferProperty, graphicsBuffer);

                setNativeData = CreateInternalSetDataDelegate();

                UnmanagedUtility.ResizePointer(ref handleBuffer, count, count * 2, Allocator.Persistent);
                UnmanagedUtility.ResizePointer(ref renderBuffer, count, count * 2, Allocator.Persistent);
            }
        }

        private Action<IntPtr, int, int, int, int> CreateInternalSetDataDelegate()
        {
            Type targetType = typeof(Action<IntPtr, int, int, int, int>);
            return (Action<IntPtr, int, int, int, int>)Delegate.CreateDelegate(targetType, graphicsBuffer, setNativeDataMethod);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Count = 0;
        }

        public void Dispose()
        {
            UnmanagedUtility.Free(ref handleBuffer, Allocator.Persistent);
            UnmanagedUtility.Free(ref renderBuffer, Allocator.Persistent);
            graphicsBuffer.Dispose();
        }
    }
}