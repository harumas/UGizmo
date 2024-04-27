using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class WireCircleAsset : GizmoAsset<WireCircle, PrimitiveData>
    {
        public override string MeshName => "WireCircle";
        public override string MaterialName => "CommonWire";
    }

    public sealed class WireCircle : GizmoRenderer<PrimitiveData>
    {
        public override int RenderQueue { get; protected set; } = 3000;
    }
}