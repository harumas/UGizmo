using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace UGizmo
{
    public interface IGizmoUpdater
    {
        JobHandle CreateJobHandle();
        void Render();
    }

    public abstract class GizmoRenderer<TJobData> : IGizmoUpdater where TJobData : unmanaged
    {
        protected GizmoBatchRendererGroup BatchRendererGroup;
        protected NativeArray<TJobData> JobData;

        protected const int MaxInstanceCount = 8192;

        protected int RenderCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        public void Initialize(Mesh mesh, Material material)
        {
            BatchRendererGroup = new GizmoBatchRendererGroup(mesh, material, MaxInstanceCount);
            JobData = new NativeArray<TJobData>(MaxInstanceCount, Allocator.Persistent);
        }

        public abstract JobHandle CreateJobHandle();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TJobData data)
        {
            if (RenderCount >= JobData.Length)
            {
                return;
            }

            JobData[RenderCount++] = data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render()
        {
            BatchRendererGroup.UploadGpuData(RenderCount);
            RenderCount = 0;
        }

        public void Dispose()
        {
            JobData.Dispose();
            BatchRendererGroup.Dispose();
        }
    }
}