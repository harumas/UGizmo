using UGizmo.Extension.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace UGizmo.Extension
{
    public sealed class WireSphereAsset : GizmoAsset<WireSphere, PrimitiveData>
    {
        public override string MeshName => "WireSphere";
        public override string MaterialName => "Common";
    }

    public sealed unsafe class WireSphere : GizmoRenderer<PrimitiveData>
    {
        public override JobHandle CreateJobHandle()
        {
            fixed (RenderData* buffer = RenderBuffer.AsSpan())
            {
                var createJob = new CreatePrimitiveObjectJob()
                {
                    GizmoDataPtr = (PrimitiveData*)JobData.GetUnsafeReadOnlyPtr(),
                    Result = buffer
                };
                
                return createJob.Schedule(InstanceCount, 16);
            }
        }
    }
}