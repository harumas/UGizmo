using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace UGizmos
{
    public interface IGizmoUpdater
    {
        JobHandle CreateJobHandle();
        void Render();
    }

    public sealed unsafe class GizmoRenderer<TCustom> : IGizmoUpdater where TCustom : unmanaged
    {
        private readonly GizmoBatchRendererGroup<TCustom> batchRendererGroup;
        private readonly int maxInstanceCount;
        private NativeArray<GizmoData<TCustom>> gizmoData;
        private int count;

        public GizmoRenderer(Mesh mesh, Material material, int maxInstanceCount)
        {
            this.maxInstanceCount = maxInstanceCount;
            batchRendererGroup = new GizmoBatchRendererGroup<TCustom>(mesh, material, maxInstanceCount);
            gizmoData = new NativeArray<GizmoData<TCustom>>(maxInstanceCount, Allocator.Persistent);
        }

        public JobHandle CreateJobHandle()
        {
            var systemBuffer = batchRendererGroup.GetBuffer();

            fixed (void* buffer = systemBuffer)
            {
                var createJob = new CreateRenderDataJob<TCustom>()
                {
                    GizmoDataPtr = (GizmoData<TCustom>*)gizmoData.GetUnsafeReadOnlyPtr(),
                    MaxInstanceCount = maxInstanceCount,
                    Result = buffer
                };

                return createJob.Schedule(gizmoData.Length, 16);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in GizmoData<TCustom> data)
        {
            if (count >= gizmoData.Length)
            {
                return;
            }

            gizmoData[count++] = data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render()
        {
            batchRendererGroup.UploadGpuData(count);
            count = 0;
        }

        public void Dispose()
        {
            gizmoData.Dispose();
            batchRendererGroup.Dispose();
        }
    }
}