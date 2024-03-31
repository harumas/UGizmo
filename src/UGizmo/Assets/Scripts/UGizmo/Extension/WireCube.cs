using UGizmo.Extension.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class WireCubeAsset : GizmoAsset<WireCube, PrimitiveData>
    {
        public override string MeshName => "WireCube";
        public override string MaterialName => "Common";
    }

    public sealed unsafe class WireCube : GizmoRenderer<PrimitiveData>
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