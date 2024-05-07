using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UGizmo.Internal
{
    public interface IGizmoDrawer : IDisposable
    {
        int RenderQueue { get; }
        void Draw(CommandBuffer commandBuffer);
        void DrawWithCamera(Camera camera);
        void UploadGpuData();
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

            //Swap buffer
            SharedGizmoBuffer<TJobData>.GetSharedBuffer().Swap();
            bufferCount = GizmoDrawBuffer.Count;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawWithCamera(Camera camera)
        {
            Graphics.DrawMeshInstancedProcedural(mesh, 0, material, new Bounds(Vector3.zero, Vector3.one * 10000), bufferCount, GizmoDrawBuffer.GetPropertyBlock(), ShadowCastingMode.Off, false, 0, camera, LightProbeUsage.Off);
        }

        public void Dispose()
        {
            GizmoDrawBuffer.Dispose();
        }
    }
}