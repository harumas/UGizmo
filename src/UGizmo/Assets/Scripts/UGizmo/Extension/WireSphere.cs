using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class WireSphereAsset : GizmoAsset<WireSphere, PrimitiveData>
    {
        public override string MeshName => "WireSphere";
        public override string MaterialName => "CommonWire";
    }

    public sealed class WireSphere : GizmoRenderer<PrimitiveData>
    {
        public override int RenderQueue { get; protected set; } = 3000;
    }
}