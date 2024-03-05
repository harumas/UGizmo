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
        protected int MaxInstanceCount = 8192;
        protected int RenderPerInstance = 1;

        protected int InstanceCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        public void Initialize(Mesh mesh, Material material)
        {
            BatchRendererGroup = new GizmoBatchRendererGroup(mesh, material, MaxInstanceCount * RenderPerInstance);
            JobData = new NativeArray<TJobData>(MaxInstanceCount, Allocator.Persistent);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render()
        {
            BatchRendererGroup.UploadGpuData(MaxInstanceCount * RenderPerInstance);
            InstanceCount = 0;
        }

        public virtual void Dispose()
        {
            JobData.Dispose();
            BatchRendererGroup.Dispose();
        }
    }
}