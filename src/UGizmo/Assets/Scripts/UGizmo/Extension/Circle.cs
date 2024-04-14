using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class CircleAsset : GizmoAsset<Circle, PrimitiveData>
    {
        public override string MeshName => "Circle";
        public override string MaterialName => "CommonMeshCullOff";
    }

    public sealed unsafe class Circle : GizmoRenderer<PrimitiveData>
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