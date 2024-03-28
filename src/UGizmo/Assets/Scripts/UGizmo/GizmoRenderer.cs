using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace UGizmo
{
    public interface IGizmoUpdater : IDisposable
    {
        JobHandle CreateJobHandle(int frameDivision);
        void Render(CommandBuffer commandBuffer);
    }

    public abstract class GizmoRenderer<TJobData> : IGizmoUpdater where TJobData : unmanaged
    {
        protected NativeArray<TJobData> JobData;
        protected NativeArray<RenderData> RenderBuffer;
        protected int MaxInstanceCount = 8192;
        protected int RenderPerInstance = 1;

        private Mesh mesh;
        private Material material;
        private GraphicsBuffer graphicsBuffer;
        private int maxRenderingCount;
        private static readonly int renderBuffer = Shader.PropertyToID("_RenderBuffer");

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
            maxRenderingCount = MaxInstanceCount * RenderPerInstance;

            JobData = new NativeArray<TJobData>(MaxInstanceCount, Allocator.Persistent);
            RenderBuffer = new NativeArray<RenderData>(maxRenderingCount, Allocator.Persistent);

            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, maxRenderingCount, Marshal.SizeOf<RenderData>());
            material.SetBuffer(renderBuffer, graphicsBuffer);
        }

        public abstract JobHandle CreateJobHandle(int frameDivision);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TJobData data)
        {
            if (InstanceCount >= JobData.Length)
            {
                return;
            }

            JobData[InstanceCount++] = data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render(CommandBuffer commandBuffer)
        {
            int renderCount = InstanceCount * RenderPerInstance;

            if (renderCount == 0)
            {
                return;
            }

            graphicsBuffer.SetData(RenderBuffer, 0, 0, renderCount);
            commandBuffer.DrawMeshInstancedProcedural(mesh, 0, material, -1, renderCount);
            Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            InstanceCount = 0;
        }

        public virtual void Dispose()
        {
            JobData.Dispose();
            RenderBuffer.Dispose();
            graphicsBuffer.Dispose();
        }
    }
}