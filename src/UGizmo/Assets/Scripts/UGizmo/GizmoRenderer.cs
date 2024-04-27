using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UGizmo.Extension;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace UGizmo
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

        private NativeArray<int> handleData;
        private NativeArray<RenderData> renderBuffer;
        private int maxInstanceCount = 8192;

        private Mesh mesh;
        private Material material;
        private GraphicsBuffer graphicsBuffer;
        private SharedGizmoBuffer<TJobData> gizmoBuffer;
        private MaterialPropertyBlock propertyBlock;
        private static readonly int renderBufferProperty = Shader.PropertyToID("_RenderBuffer");

        protected int InstanceCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        public void Initialize(Mesh mesh, Material material)
        {
            this.mesh = mesh;
            this.material = material;

            handleData = new NativeArray<int>(maxInstanceCount, Allocator.Persistent);
            renderBuffer = new NativeArray<RenderData>(maxInstanceCount, Allocator.Persistent);

            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, maxInstanceCount, Marshal.SizeOf<RenderData>());
            gizmoBuffer = SharedGizmoBuffer<TJobData>.GetSharedBuffer();
            propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetBuffer(renderBufferProperty, graphicsBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TJobData data)
        {
            handleData[InstanceCount++] = gizmoBuffer.Add(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TJobData* Reserve(int count)
        {
            int offset = InstanceCount;
            InstanceCount += count;
            (int start, int length) handle = gizmoBuffer.Reserve(count, out TJobData* ptr);

            for (int i = 0; i < handle.length; i++)
            {
                handleData[offset + i] = handle.start + i;
            }
            
            return ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render(CommandBuffer commandBuffer)
        {
            if (InstanceCount == 0)
            {
                return;
            }

            gizmoBuffer.SetRenderData(handleData, renderBuffer, InstanceCount);
            graphicsBuffer.SetData(renderBuffer, 0, 0, InstanceCount);
            commandBuffer.DrawMeshInstancedProcedural(mesh, 0, material, -1, InstanceCount, propertyBlock);
            Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Clear()
        {
            InstanceCount = 0;
        }

        public void Dispose()
        {
            renderBuffer.Dispose();
            handleData.Dispose();
            graphicsBuffer.Dispose();
        }
    }
}