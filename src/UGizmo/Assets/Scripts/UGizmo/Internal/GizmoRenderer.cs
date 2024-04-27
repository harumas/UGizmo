using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

namespace UGizmo.Internal
{
    public interface IGizmoRenderer : IDisposable
    {
        int RenderQueue { get; }
        void Render(CommandBuffer commandBuffer);
    }

    [BurstCompile]
    public abstract unsafe class GizmoRenderer<TJobData> : IGizmoRenderer where TJobData : unmanaged
    {
        public virtual int RenderQueue { get; protected set; } = 1000;

        private int* handleBuffer;
        private RenderData* renderBuffer;
        private const int InitialCapacity = 8192;
        private int instanceCount;

        private Mesh mesh;
        private Material material;
        private GraphicsBuffer graphicsBuffer;
        private SharedGizmoBuffer<TJobData> gizmoBuffer;
        private MaterialPropertyBlock propertyBlock;
        private Action<IntPtr, int, int, int, int> setNativeData;
        private static readonly int renderBufferProperty = Shader.PropertyToID("_RenderBuffer");
        private static readonly MethodInfo setNativeDataMethod;

        static GizmoRenderer()
        {
            setNativeDataMethod = typeof(GraphicsBuffer).GetRuntimeMethods().First(method => method.Name == "InternalSetNativeData");
        }

        public void Initialize(Mesh mesh, Material material)
        {
            this.mesh = mesh;
            this.material = material;

            handleBuffer = UnmanagedUtility.Malloc<int>(InitialCapacity, Allocator.Persistent);
            renderBuffer = UnmanagedUtility.Malloc<RenderData>(InitialCapacity, Allocator.Persistent);

            gizmoBuffer = SharedGizmoBuffer<TJobData>.GetSharedBuffer();
            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, InitialCapacity, Marshal.SizeOf<RenderData>());
            propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetBuffer(renderBufferProperty, graphicsBuffer);

            setNativeData = (Action<IntPtr, int, int, int, int>)Delegate.CreateDelegate(typeof(Action<IntPtr, int, int, int, int>),
                graphicsBuffer,
                setNativeDataMethod);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TJobData data)
        {
            CheckCapacity();

            handleBuffer[instanceCount++] = gizmoBuffer.Add(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(TJobData* data, int count)
        {
            CheckCapacity();

            int offset = instanceCount;
            instanceCount += count;
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

            int offset = instanceCount;
            instanceCount += count;

            (int start, int length) handle = gizmoBuffer.Reserve(count, out TJobData* ptr);

            for (int i = 0; i < handle.length; i++)
            {
                handleBuffer[offset + i] = handle.start + i;
            }

            return ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render(CommandBuffer commandBuffer)
        {
            if (instanceCount == 0)
            {
                return;
            }

            gizmoBuffer.SetRenderData(handleBuffer, renderBuffer, instanceCount);
            setNativeData((IntPtr)renderBuffer, 0, 0, instanceCount, sizeof(RenderData));
            commandBuffer.DrawMeshInstancedProcedural(mesh, 0, material, -1, instanceCount, propertyBlock);
            Clear();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckCapacity()
        {
            if (instanceCount >= graphicsBuffer.count)
            {
                int count = graphicsBuffer.count;
                graphicsBuffer?.Dispose();
                graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count * 2, Marshal.SizeOf<RenderData>());
                propertyBlock.SetBuffer(renderBufferProperty, graphicsBuffer);

                setNativeData = (Action<IntPtr, int, int, int, int>)Delegate.CreateDelegate(typeof(Action<IntPtr, int, int, int, int>),
                    graphicsBuffer,
                    setNativeDataMethod);

                UnmanagedUtility.ResizePointer(ref handleBuffer, count, count * 2, Allocator.Persistent);
                UnmanagedUtility.ResizePointer(ref renderBuffer, count, count * 2, Allocator.Persistent);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Clear()
        {
            instanceCount = 0;
        }

        public void Dispose()
        {
            UnsafeUtility.Free(handleBuffer, Allocator.Persistent);
            UnsafeUtility.Free(renderBuffer, Allocator.Persistent);
            graphicsBuffer.Dispose();
        }
    }
}