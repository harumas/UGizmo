using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class WireSphereAsset : GizmoAsset<WireSphere, PrimitiveData>
    {
        public override string MeshName => "WireSphere";
        public override string MaterialName => "CommonWire";
    }

    public sealed unsafe class WireSphere : GizmoRenderer<PrimitiveData>
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