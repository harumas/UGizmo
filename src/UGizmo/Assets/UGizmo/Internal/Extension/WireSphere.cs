using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class WireSphereAsset : GizmoAsset<WireSphere, PrimitiveData>
    {
        public override string MeshName => "WireSphere";
        public override string MaterialName => "CommonWire";
    }

    internal sealed class WireSphere : GizmoDrawer<PrimitiveData>
    {
        public override int RenderQueue => 3000;
    }
}