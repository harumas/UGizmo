using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class ConeAsset : GizmoAsset<Cone, ConeData>
    {
        public override string MeshName => "Cone";
        public override string MaterialName => "CommonMesh";
    }

    internal sealed class Cone : GizmoRenderer<ConeData>
    {
    }
}