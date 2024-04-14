using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class PlaneAsset : GizmoAsset<Plane, PrimitiveData>
    {
        public override string MeshName => "Plane";
        public override string MaterialName => "CommonMeshCullOff";
    }

    public sealed unsafe class Plane : GizmoRenderer<PrimitiveData>
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