using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class WireCubeAsset : GizmoAsset<WireCube, PrimitiveData>
    {
        public override string MeshName => "WireCube";
        public override string MaterialName => "CommonWire";
    }

    public sealed unsafe class WireCube : GizmoRenderer<PrimitiveData>
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