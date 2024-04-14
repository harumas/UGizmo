using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class HemisphereAsset : GizmoAsset<Hemisphere, PrimitiveData>
    {
        public override string MeshName => "Hemisphere";
        public override string MaterialName => "CommonMesh";
    }

    public sealed unsafe class Hemisphere : GizmoRenderer<PrimitiveData>
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