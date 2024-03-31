using UGizmo.Extension.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace UGizmo.Extension
{
    public sealed class FrustumAsset : GizmoAsset<Frustum, FrustumData>
    {
        public override string MeshName => "WireLine";
        public override string MaterialName => "Common";
    }

    public sealed unsafe class Frustum : GizmoRenderer<FrustumData>
    {
        private NativeArray<FrustumLineData> frustumLineData;

        public Frustum() : base()
        {
            RenderPerInstance = 12;
            frustumLineData = new NativeArray<FrustumLineData>(MaxInstanceCount * RenderPerInstance, Allocator.Persistent);
        }

        public override JobHandle CreateJobHandle()
        {
            JobHandle prepareDataJob = new CreateFrustumJob()
            {
                GizmoDataPtr = (FrustumData*)JobData.GetUnsafeReadOnlyPtr(),
                Result = (FrustumLineData*)frustumLineData.GetUnsafePtr()
            }.Schedule(InstanceCount, 8);

            fixed (RenderData* buffer = RenderBuffer.AsSpan())
            {
                CreateFrustumLineJob createJob = new CreateFrustumLineJob()
                {
                    GizmoDataPtr = (FrustumLineData*)frustumLineData.GetUnsafeReadOnlyPtr(),
                    Result = buffer
                };

                return createJob.Schedule(InstanceCount * RenderPerInstance, 16, prepareDataJob);
            }
        }

        public override void Dispose()
        {
            frustumLineData.Dispose();
            base.Dispose();
        }
    }
}