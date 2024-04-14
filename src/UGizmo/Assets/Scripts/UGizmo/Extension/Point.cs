using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class PointAsset : GizmoAsset<Point, PrimitiveData>
    {
        public override string MeshName => "Point";
        public override string MaterialName => "CommonMeshFront";
    }

    public sealed unsafe class Point : GizmoRenderer<PrimitiveData>
    {
        public override int RenderQueue { get; protected set; } = 2000;

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