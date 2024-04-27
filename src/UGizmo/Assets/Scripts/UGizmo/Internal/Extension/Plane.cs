using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class PlaneAsset : GizmoAsset<Plane, PrimitiveData>
    {
        public override string MeshName => "Plane";
        public override string MaterialName => "CommonMeshCullOff";
    }

    internal sealed class Plane : GizmoRenderer<PrimitiveData>
    {
    }
}