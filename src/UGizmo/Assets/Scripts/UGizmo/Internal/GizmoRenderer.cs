using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
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
    internal abstract class GizmoRenderer<TJobData> : IGizmoRenderer where TJobData : unmanaged
    {
        public virtual int RenderQueue => 1000;
        public GizmoRenderBuffer<TJobData> RenderBuffer { get; private set; }

        private Mesh mesh;
        private Material material;

        public void Initialize(Mesh mesh, Material material)
        {
            this.mesh = mesh;
            this.material = material;

            RenderBuffer = new GizmoRenderBuffer<TJobData>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render(CommandBuffer commandBuffer)
        {
            if (RenderBuffer.Count == 0)
            {
                return;
            }

            RenderBuffer.UploadGpuData();
            commandBuffer.DrawMeshInstancedProcedural(mesh, 0, material, -1, RenderBuffer.Count, RenderBuffer.GetPropertyBlock());
            RenderBuffer.Clear();
        }

        public void Dispose()
        {
            RenderBuffer.Dispose();
        }
    }
}