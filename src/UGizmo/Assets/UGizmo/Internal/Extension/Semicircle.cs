using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class SemicircleAsset : GizmoAsset<Semicircle, PrimitiveData>
    {
        public override string MeshName => "Semicircle";
        public override string MaterialName => "CommonMeshCullOff";
    }

    internal sealed class Semicircle : GizmoRenderer<PrimitiveData>
    {
    }
}