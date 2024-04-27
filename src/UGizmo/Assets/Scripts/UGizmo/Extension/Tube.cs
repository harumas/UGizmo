using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class TubeAsset : GizmoAsset<Tube, PrimitiveData>
    {
        public override string MeshName => "Tube";
        public override string MaterialName => "CommonMesh";
    }

    public sealed class Tube : GizmoRenderer<PrimitiveData>
    {
    }
}