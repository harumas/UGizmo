using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class PointAsset : GizmoAsset<Point, PrimitiveData>
    {
        public override string MeshName => "Point";
        public override string MaterialName => "CommonMeshFront";
    }

    internal sealed class Point : GizmoRenderer<PrimitiveData>
    {
        public override int RenderQueue { get; protected set; } = 2000;
    }
}