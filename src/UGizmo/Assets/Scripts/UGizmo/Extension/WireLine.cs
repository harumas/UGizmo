using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class WireLineAsset : GizmoAsset<WireLine, LineData>
    {
        public override string MeshName => "WireLine";
        public override string MaterialName => "CommonWire";
    }

    public sealed class WireLine : GizmoRenderer<LineData>
    {
        public override int RenderQueue { get; protected set; } = 3000;
    }
}