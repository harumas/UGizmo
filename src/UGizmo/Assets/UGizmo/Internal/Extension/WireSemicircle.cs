using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class WireSemicircleAsset : GizmoAsset<WireSemicircle, PrimitiveData>
    {
        public override string MeshName => "WireSemicircle";
        public override string MaterialName => "CommonWire";
    }

    internal sealed class WireSemicircle : GizmoDrawer<PrimitiveData>
    {
        public override int RenderQueue => 3000;
    }
}