using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo
{
    public sealed class TubeAsset : GizmoAsset<Tube, PrimitiveData>
    {
        public override string MeshName => "Tube";
        public override string MaterialName => "CommonMesh";
    }

    public sealed unsafe class Tube : GizmoRenderer<PrimitiveData>
    {
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