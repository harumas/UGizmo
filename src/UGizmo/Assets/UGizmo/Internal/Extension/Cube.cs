using UGizmo.Internal.Extension.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed class CubeAsset : GizmoAsset<Cube, PrimitiveData>
    {
        public override string MeshName => "Cube";
        public override string MaterialName => "CommonMesh";
    }

    internal sealed class Cube : GizmoDrawer<PrimitiveData>
    {
    }
}