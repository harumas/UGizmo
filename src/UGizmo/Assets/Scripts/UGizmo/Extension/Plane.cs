using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class PlaneAsset : GizmoAsset<Plane, PrimitiveData>
    {
        public override string MeshName => "Plane";
        public override string MaterialName => "CommonMeshCullOff";
    }

    public sealed class Plane : GizmoRenderer<PrimitiveData>
    {
    }
}