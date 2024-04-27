using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class WireSemicircleAsset : GizmoAsset<WireSemicircle, PrimitiveData>
    {
        public override string MeshName => "WireSemicircle";
        public override string MaterialName => "CommonWire";
    }

    public sealed class WireSemicircle : GizmoRenderer<PrimitiveData>
    {
        public override int RenderQueue { get; protected set; } = 3000;
    }
}