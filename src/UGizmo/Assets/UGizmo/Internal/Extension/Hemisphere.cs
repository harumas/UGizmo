using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class HemisphereAsset : GizmoAsset<Hemisphere, PrimitiveData>
    {
        public override string MeshName => "Hemisphere";
        public override string MaterialName => "CommonMesh";
    }

    internal sealed class Hemisphere : GizmoRenderer<PrimitiveData>
    {
    }
}