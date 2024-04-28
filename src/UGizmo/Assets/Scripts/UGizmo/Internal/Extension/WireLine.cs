using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class WireLineAsset : GizmoAsset<WireLine, LineData>
    {
        public override string MeshName => "WireLine";
        public override string MaterialName => "CommonWire";
    }

    internal sealed class WireLine : GizmoRenderer<LineData>
    {
        public override int RenderQueue => 3000;
    }
}