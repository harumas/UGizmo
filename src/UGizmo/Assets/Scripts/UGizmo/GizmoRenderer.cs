using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace UGizmo
{
    public interface IGizmoUpdater
    {
        JobHandle CreateJobHandle(int frameDivision);
        void Render(int frameDivision);
        void Reset();
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
        public void Render(int frameDivision)
        {
            BatchRendererGroup.UploadGpuData(InstanceCount * RenderPerInstance / frameDivision);
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
            BatchRendererGroup.Dispose();
        }
    }
}