using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class CylinderAsset : GizmoAsset<Cylinder, PrimitiveData>
    {
        public override string MeshName => "Cylinder";
        public override string MaterialName => "CommonMesh";
    }

    internal sealed class Cylinder : GizmoDrawer<PrimitiveData>
    {
    }
}