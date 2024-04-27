using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class CircleAsset : GizmoAsset<Circle, PrimitiveData>
    {
        public override string MeshName => "Circle";
        public override string MaterialName => "CommonMeshCullOff";
    }

    public sealed class Circle : GizmoRenderer<PrimitiveData>
    {
    }
}