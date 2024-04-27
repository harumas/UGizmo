using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class SemicircleAsset : GizmoAsset<Semicircle, PrimitiveData>
    {
        public override string MeshName => "Semicircle";
        public override string MaterialName => "CommonMeshCullOff";
    }

    public sealed class Semicircle : GizmoRenderer<PrimitiveData>
    {
    }
}