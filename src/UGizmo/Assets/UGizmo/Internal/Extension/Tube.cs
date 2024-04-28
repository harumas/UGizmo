using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class TubeAsset : GizmoAsset<Tube, PrimitiveData>
    {
        public override string MeshName => "Tube";
        public override string MaterialName => "CommonMesh";
    }

    internal sealed class Tube : GizmoRenderer<PrimitiveData>
    {
    }
}