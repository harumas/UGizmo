using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class SphereAsset : GizmoAsset<Sphere, PrimitiveData>
    {
        public override string MeshName => "Sphere";
        public override string MaterialName => "CommonMesh";
    }

    internal sealed class Sphere : GizmoRenderer<PrimitiveData>
    {
    }
}