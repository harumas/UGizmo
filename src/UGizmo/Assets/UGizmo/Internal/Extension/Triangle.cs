using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class TriangleAsset : GizmoAsset<Triangle, PrimitiveData>
    {
        public override string MeshName => "Triangle";
        public override string MaterialName => "CommonMeshCullOff";
    }

    internal sealed class Triangle : GizmoDrawer<PrimitiveData>
    {
    }
}