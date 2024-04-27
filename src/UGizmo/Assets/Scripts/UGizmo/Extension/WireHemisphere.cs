using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class WireHemisphereAsset : GizmoAsset<WireHemisphere, PrimitiveData>
    {
        public override string MeshName => "WireHemisphere";
        public override string MaterialName => "CommonWire";
    }

    public sealed class WireHemisphere : GizmoRenderer<PrimitiveData>
    {
        public override int RenderQueue { get; protected set; } = 3000;
    }
}