using UGizmos;
using UGizmos.Extension;
using Unity.Jobs;

namespace UGizmos.Extension
{
    public class WireSphere : GizmoElement<WireSphere, NoCustom>
    {
        public override string MeshPath => "Meshes/WireSphere";
        public override string MaterialPath => "Materials/Common";
    }
}