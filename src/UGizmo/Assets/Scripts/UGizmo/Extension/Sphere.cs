using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class SphereAsset : GizmoAsset<Sphere, PrimitiveData>
    {
        public override string MeshName => "Sphere";
        public override string MaterialName => "CommonMesh";
    }

    public sealed class Sphere : GizmoRenderer<PrimitiveData>
    {
    }
}