using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class WireCubeAsset : GizmoAsset<WireCube, PrimitiveData>
    {
        public override string MeshName => "WireCube";
        public override string MaterialName => "CommonWire";
    }

    internal sealed class WireCube : GizmoRenderer<PrimitiveData>
    {
        public override int RenderQueue => 3000;
    }
}