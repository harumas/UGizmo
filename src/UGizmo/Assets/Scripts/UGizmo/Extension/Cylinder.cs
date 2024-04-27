using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class CylinderAsset : GizmoAsset<Cylinder, PrimitiveData>
    {
        public override string MeshName => "Cylinder";
        public override string MaterialName => "CommonMesh";
    }

    public sealed class Cylinder : GizmoRenderer<PrimitiveData>
    {
    }
}