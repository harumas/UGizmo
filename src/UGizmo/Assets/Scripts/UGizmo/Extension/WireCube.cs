using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class WireCubeAsset : GizmoAsset<WireCube, PrimitiveData>
    {
        public override string MeshName => "WireCube";
        public override string MaterialName => "CommonWire";
    }

    public sealed class WireCube : GizmoRenderer<PrimitiveData>
    {
        public override int RenderQueue { get; protected set; } = 3000;
    }
}