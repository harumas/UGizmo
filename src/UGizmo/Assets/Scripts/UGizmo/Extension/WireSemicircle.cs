using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class WireSemicircleAsset : GizmoAsset<WireSemicircle, PrimitiveData>
    {
        public override string MeshName => "WireSemicircle";
        public override string MaterialName => "CommonWire";
    }

    public sealed unsafe class WireSemicircle : GizmoRenderer<PrimitiveData>
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