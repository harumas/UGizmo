using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class CylinderAsset : GizmoAsset<Cylinder, PrimitiveData>
    {
        public override string MeshName => "Cylinder";
        public override string MaterialName => "CommonMesh";
    }

    public sealed unsafe class Cylinder : GizmoRenderer<PrimitiveData>
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