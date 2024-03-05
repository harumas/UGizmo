using UGizmo.Extension.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class WireLineAsset : GizmoAsset<WireLine, LineData>
    {
        public override string MeshName => "WireLine";
        public override string MaterialName => "Common";
    }

    public sealed unsafe class WireLine : GizmoRenderer<LineData>
    {
        public override JobHandle CreateJobHandle()
        {
            var systemBuffer = BatchRendererGroup.GetBuffer();

            fixed (void* buffer = systemBuffer)
            {
                var createJob = new CreateWireLineJob()
                {
                    GizmoDataPtr = (LineData*)JobData.GetUnsafeReadOnlyPtr(),
                    MaxInstanceCount = MaxInstanceCount,
                    Result = buffer
                };

                return createJob.Schedule(InstanceCount, 16);
            }
        }
    }
}