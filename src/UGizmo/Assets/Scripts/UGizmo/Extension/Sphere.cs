using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class SphereAsset : GizmoAsset<Sphere, PrimitiveData>
    {
        public override string MeshName => "Sphere";
        public override string MaterialName => "CommonMesh";
    }

    public sealed unsafe class Sphere : GizmoRenderer<PrimitiveData>
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