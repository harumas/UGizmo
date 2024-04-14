using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class ConeAsset : GizmoAsset<Cone, ConeData>
    {
        public override string MeshName => "Cone";
        public override string MaterialName => "CommonMesh";
    }

    public sealed unsafe class Cone : GizmoRenderer<ConeData>
    {
        public override JobHandle CreateJobHandle()
        {
            var createJob = new CreateConeJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = RenderBufferPtr
            };

            return createJob.Schedule(InstanceCount, 16, Dependency);
        }
    }
}