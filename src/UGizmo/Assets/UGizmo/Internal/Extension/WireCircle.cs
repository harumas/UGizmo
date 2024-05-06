using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class WireCircleAsset : GizmoAsset<WireCircle, PrimitiveData>
    {
        public override string MeshName => "WireCircle";
        public override string MaterialName => "CommonWire";
    }

    internal sealed class WireCircle : GizmoDrawer<PrimitiveData>
    {
        public override int RenderQueue => 3000;
    }
}