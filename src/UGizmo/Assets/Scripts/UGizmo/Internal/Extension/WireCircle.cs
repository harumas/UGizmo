using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class WireCircleAsset : GizmoAsset<WireCircle, PrimitiveData>
    {
        public override string MeshName => "WireCircle";
        public override string MaterialName => "CommonWire";
    }

    internal sealed class WireCircle : GizmoRenderer<PrimitiveData>
    {
        public override int RenderQueue { get; protected set; } = 3000;
    }
}