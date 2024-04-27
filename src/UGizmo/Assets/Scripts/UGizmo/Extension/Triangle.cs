using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class TriangleAsset : GizmoAsset<Triangle, PrimitiveData>
    {
        public override string MeshName => "Triangle";
        public override string MaterialName => "CommonMeshCullOff";
    }

    public sealed class Triangle : GizmoRenderer<PrimitiveData>
    {
    }
}