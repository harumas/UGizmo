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
            fixed (RenderData* buffer = RenderBuffer.AsSpan())
            {
                var createJob = new CreateWireLineJob()
                {
                    GizmoDataPtr = (LineData*)JobData.GetUnsafeReadOnlyPtr(),
                    Result = buffer
                };

                return createJob.Schedule(InstanceCount, 16);
            }
        }
    }
}