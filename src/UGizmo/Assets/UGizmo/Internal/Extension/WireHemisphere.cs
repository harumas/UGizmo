using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class WireHemisphereAsset : GizmoAsset<WireHemisphere, PrimitiveData>
    {
        public override string MeshName => "WireHemisphere";
        public override string MaterialName => "CommonWire";
    }

    internal sealed class WireHemisphere : GizmoDrawer<PrimitiveData>
    {
        public override int RenderQueue => 3000;
    }
}