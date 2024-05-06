using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class PointAsset : GizmoAsset<Point, PrimitiveData>
    {
        public override string MeshName => "Point";
        public override string MaterialName => "CommonMeshFront";
    }

    internal sealed class Point : GizmoDrawer<PrimitiveData>
    {
        public override int RenderQueue => 2000;
    }
}