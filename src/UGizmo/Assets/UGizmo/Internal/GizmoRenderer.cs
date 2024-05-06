using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Rendering;

namespace UGizmo.Internal
{
    public interface IGizmoDrawer : IDisposable
    {
        int RenderQueue { get; }
        void Draw(CommandBuffer commandBuffer);
        void UploadGpuData();
        void SwapBuffer();
    }

    [BurstCompile]
    internal abstract class GizmoDrawer<TJobData> : IGizmoDrawer where TJobData : unmanaged
    {
        public virtual int RenderQueue => 1000;
        public GizmoDrawBuffer<TJobData> GizmoDrawBuffer { get; private set; }

        private Mesh mesh;
        private Material material;

        private int bufferCount;

        public void Initialize(Mesh mesh, Material material)
        {
            this.mesh = mesh;
            this.material = material;

            GizmoDrawBuffer = new GizmoDrawBuffer<TJobData>();
        }

        public void UploadGpuData()
        {
            GizmoDrawBuffer.UploadGpuData();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(CommandBuffer commandBuffer)
        {
            if (bufferCount == 0)
            {
                return;
            }

            commandBuffer.DrawMeshInstancedProcedural(mesh, 0, material, -1, bufferCount, GizmoDrawBuffer.GetPropertyBlock());
            GizmoDrawBuffer.Clear();
        }
        
        public void SwapBuffer()
        {
            SharedGizmoBuffer<TJobData>.GetSharedBuffer().Swap();
            bufferCount = GizmoDrawBuffer.Count;
        }

        public void Dispose()
        {
            GizmoDrawBuffer.Dispose();
        }
    }
}