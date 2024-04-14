using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo
{
    public sealed class WireCircleAsset : GizmoAsset<WireCircle, PrimitiveData>
    {
        public override string MeshName => "WireCircle";
        public override string MaterialName => "CommonWire";
    }

    public sealed unsafe class WireCircle : GizmoRenderer<PrimitiveData>
    {
        public override int RenderQueue { get; protected set; } = 3000;
        
        public override JobHandle CreateJobHandle()
        {
            var createJob = new CreatePrimitiveJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = RenderBufferPtr
            };

            return createJob.Schedule(InstanceCount, 16, Dependency);
        }
    }
}