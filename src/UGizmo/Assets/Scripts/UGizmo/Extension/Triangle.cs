using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class TriangleAsset : GizmoAsset<Triangle, PrimitiveData>
    {
        public override string MeshName => "Triangle";
        public override string MaterialName => "CommonMeshCullOff";
    }

    public sealed unsafe class Triangle : GizmoRenderer<PrimitiveData>
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