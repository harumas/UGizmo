using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using UGizmo.Internal.Utility;
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
    internal abstract unsafe class GizmoDrawer<TJobData> : IGizmoDrawer, IContinuousGizmoReceiver<TJobData> where TJobData : unmanaged
    {
        public virtual int RenderQueue => 1000;

        private static readonly Bounds DrawBounds = new Bounds(Vector3.zero, Vector3.one * 10000);

        private GizmoDrawBuffer<TJobData> drawBuffer;
        private ContinuousGizmoBuffer<TJobData, GizmoDrawer<TJobData>> continuousGizmoBuffer;
        private Mesh mesh;
        private Material material;
        private string meshName;
        private string materialName;

        private int bufferCount;

        public void Initialize(Mesh mesh, Material material, string meshName, string materialName)
        {
            this.mesh = mesh;
            this.material = material;
            this.meshName = meshName;
            this.materialName = materialName;

            drawBuffer = new GizmoDrawBuffer<TJobData>();
            continuousGizmoBuffer = new ContinuousGizmoBuffer<TJobData, GizmoDrawer<TJobData>>(this);
        }

        // Unity can destroy this material outside of our control (e.g. shader recompilation during a Build), so re-resolve it lazily before drawing.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureMaterialIsValid()
        {
            if (material != null)
            {
                return;
            }

            (_, material) = AssetUtility.CreateMeshAndMaterial(meshName, materialName);
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
        public int Reserve(int count)
        {
            return drawBuffer.Reserve(count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TJobData* GetReservedPointer(int start)
        {
            return drawBuffer.GetReservedPointer(start);
        }

        public void UploadGpuData()
        {
            bufferCount = drawBuffer.Count;
            if (bufferCount == 0)
            {
                return;
            }

            drawBuffer.UploadGpuData();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(CommandBuffer commandBuffer)
        {
            if (bufferCount == 0)
            {
                return;
            }

            EnsureMaterialIsValid();

            commandBuffer.DrawMeshInstancedProcedural(mesh, 0, material, -1, bufferCount, drawBuffer.GetPropertyBlock());
            drawBuffer.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawWithCamera(Camera camera)
        {
            if (bufferCount == 0)
            {
                return;
            }

            EnsureMaterialIsValid();

            Graphics.DrawMeshInstancedProcedural(mesh, 0, material, DrawBounds, bufferCount,
                drawBuffer.GetPropertyBlock(), ShadowCastingMode.Off, false, 0, camera, LightProbeUsage.Off);
            drawBuffer.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFromContinuous(in TJobData data)
        {
            drawBuffer.Add(data);
        }

        public void Dispose()
        {
            drawBuffer.Dispose();
        }
    }
}