using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class ConeAsset : GizmoAsset<Cone, ConeData>
    {
        public override string MeshName => "Cone";
        public override string MaterialName => "CommonMesh";
    }

    public sealed class Cone : GizmoRenderer<ConeData>
    {
    }
}