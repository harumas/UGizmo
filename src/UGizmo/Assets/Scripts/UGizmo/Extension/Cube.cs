using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class CubeAsset : GizmoAsset<Cube, PrimitiveData>
    {
        public override string MeshName => "Cube";
        public override string MaterialName => "CommonMesh";
    }

    public sealed unsafe class Cube : GizmoRenderer<PrimitiveData>
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