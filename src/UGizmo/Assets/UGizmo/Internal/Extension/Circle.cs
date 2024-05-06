using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class CircleAsset : GizmoAsset<Circle, PrimitiveData>
    {
        public override string MeshName => "Circle";
        public override string MaterialName => "CommonMeshCullOff";
    }

    internal sealed class Circle : GizmoDrawer<PrimitiveData>
    {
    }
}