using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class PointAsset : GizmoAsset<Point, PrimitiveData>
    {
        public override string MeshName => "Point";
        public override string MaterialName => "CommonMeshFront";
    }

    public sealed class Point : GizmoRenderer<PrimitiveData>
    {
        public override int RenderQueue { get; protected set; } = 2000;
    }
}