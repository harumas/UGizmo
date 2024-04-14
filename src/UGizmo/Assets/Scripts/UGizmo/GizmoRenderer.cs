using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace UGizmo
{
    public interface IGizmoRenderer : IDisposable
    {
        int RenderQueue { get; }

        JobHandle CreateJobHandle();
        void Render(CommandBuffer commandBuffer);
    }

    [BurstCompile]
    public abstract unsafe class GizmoRenderer<TJobData> : IGizmoRenderer where TJobData : unmanaged
    {
        public virtual int RenderQueue { get; protected set; } = 1000;

        protected NativeArray<TJobData> JobData;
        protected NativeArray<RenderData> RenderBuffer;
        protected JobHandle Dependency = default;
        protected int MaxInstanceCount = 8192;

        private Mesh mesh;
        private Material material;
        private GraphicsBuffer graphicsBuffer;
        private MaterialPropertyBlock propertyBlock;
        private static readonly int renderBuffer = Shader.PropertyToID("_RenderBuffer");

        protected int InstanceCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        protected TJobData* JobDataPtr => (TJobData*)JobData.GetUnsafePtr();
        protected RenderData* RenderBufferPtr => (RenderData*)RenderBuffer.GetUnsafePtr();

        public void Initialize(Mesh mesh, Material material)
        {
            this.mesh = mesh;
            this.material = material;

            JobData = new NativeArray<TJobData>(MaxInstanceCount, Allocator.Persistent);
            RenderBuffer = new NativeArray<RenderData>(MaxInstanceCount, Allocator.Persistent);

            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, MaxInstanceCount, Marshal.SizeOf<RenderData>());
            propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetBuffer(renderBuffer, graphicsBuffer);
        }

        public abstract JobHandle CreateJobHandle();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TJobData data)
        {
            if (InstanceCount >= JobData.Length)
            {
                return;
            }

            JobData[InstanceCount++] = data;
        }

        [BurstCompile]
        public void AddRange(TJobData* data, int length)
        {
            if (InstanceCount + length - 1 >= JobData.Length)
            {
                return;
            }

            for (int i = 0; i < length; i++)
            {
                JobData[InstanceCount + i] = *(data + i);
            }

            InstanceCount += length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddDependency(JobHandle jobHandle)
        {
            Dependency = JobHandle.CombineDependencies(Dependency, jobHandle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TJobData* Reserve(int count)
        {
            if (InstanceCount + count >= JobData.Length)
            {
                throw new OutOfMemoryException();
            }

            TJobData* ptr = (TJobData*)JobData.GetUnsafePtr() + InstanceCount;
            InstanceCount += count;
            return ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render(CommandBuffer commandBuffer)
        {
            if (InstanceCount == 0)
            {
                return;
            }

            graphicsBuffer.SetData(RenderBuffer, 0, 0, InstanceCount);
            commandBuffer.DrawMeshInstancedProcedural(mesh, 0, material, -1, InstanceCount, propertyBlock);
            Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Clear()
        {
            InstanceCount = 0;
            Dependency = default;
        }

        public void Dispose()
        {
            RenderBuffer.Dispose();
            JobData.Dispose();
            graphicsBuffer.Dispose();
        }
    }
}