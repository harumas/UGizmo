using UGizmo.Extension.Jobs;

namespace UGizmo.Extension
{
    public sealed class CubeAsset : GizmoAsset<Cube, PrimitiveData>
    {
        public override string MeshName => "Cube";
        public override string MaterialName => "CommonMesh";
    }

    public sealed class Cube : GizmoRenderer<PrimitiveData>
    {
    }
}