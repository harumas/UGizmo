using UGizmo.Extension.Jobs;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace UGizmo.Extension
{
    public sealed class WireLineAsset : GizmoAsset<WireLine, LineData>
    {
        public override string MeshName => "WireLine";
        public override string MaterialName => "CommonWire";
    }

    public sealed unsafe class WireLine : GizmoRenderer<LineData>
    {
        public override int RenderQueue { get; protected set; } = 3000;

        public override JobHandle CreateJobHandle()
        {
            var createJob = new CreateWireLineJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = RenderBufferPtr
            };

            return createJob.Schedule(InstanceCount, 16, Dependency);
        }
    }
}