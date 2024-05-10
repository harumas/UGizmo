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
        void EnqueueContinuousGizmo();
        void ClearContinuousGizmo();
        void Draw(CommandBuffer commandBuffer);
        void DrawWithCamera(Camera camera);
        void UploadGpuData();
    }

    [BurstCompile]
    internal abstract unsafe class GizmoDrawer<TJobData> : IGizmoDrawer where TJobData : unmanaged
    {
        public virtual int RenderQueue => 1000;

        private GizmoDrawBuffer<TJobData> drawBuffer;
        private ContinuousGizmoBuffer<TJobData> continuousGizmoBuffer;
        private Mesh mesh;
        private Material material;

        private int bufferCount;

        public void Initialize(Mesh mesh, Material material)
        {
            this.mesh = mesh;
            this.material = material;

            drawBuffer = new GizmoDrawBuffer<TJobData>();
            continuousGizmoBuffer = new ContinuousGizmoBuffer<TJobData>(data => drawBuffer.Add(data));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TJobData jobData, float duration)
        {
            if (duration > 0f)
            {
                continuousGizmoBuffer.Add(jobData, duration);
            }
            else
            {
                drawBuffer.Add(jobData);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(TJobData* jobData, int count, float duration)
        {
            if (duration > 0f)
            {
                continuousGizmoBuffer.AddRange(jobData, count, duration);
            }
            else
            {
                drawBuffer.AddRange(jobData, count);
            }
        }

        public void EnqueueContinuousGizmo()
        {
            continuousGizmoBuffer.EnqueueAllJobData();
        }

        public void ClearContinuousGizmo()
        {
            continuousGizmoBuffer.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TJobData* Reserve(int count)
        {
            return drawBuffer.Reserve(count);
        }

        public void UploadGpuData()
        {
            drawBuffer.UploadGpuData();

            //Swap buffer
            SharedGizmoBuffer<TJobData>.GetSharedBuffer().Swap();
            bufferCount = drawBuffer.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(CommandBuffer commandBuffer)
        {
            if (bufferCount == 0)
            {
                return;
            }

            commandBuffer.DrawMeshInstancedProcedural(mesh, 0, material, -1, bufferCount, drawBuffer.GetPropertyBlock());
            drawBuffer.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawWithCamera(Camera camera)
        {
            Graphics.DrawMeshInstancedProcedural(mesh, 0, material, new Bounds(Vector3.zero, Vector3.one * 10000), bufferCount,
                drawBuffer.GetPropertyBlock(), ShadowCastingMode.Off, false, 0, camera, LightProbeUsage.Off);
        }

        public void Dispose()
        {
            drawBuffer.Dispose();
        }
    }
}