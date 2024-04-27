using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class HemisphereAsset : GizmoAsset<Hemisphere, PrimitiveData>
    {
        public override string MeshName => "Hemisphere";
        public override string MaterialName => "CommonMesh";
    }

    public sealed class Hemisphere : GizmoRenderer<PrimitiveData>
    {
    }
}