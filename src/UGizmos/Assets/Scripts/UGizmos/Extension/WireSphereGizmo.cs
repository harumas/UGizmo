using UGizmos;
using UGizmos.Extension;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace UGizmos.Extension
{
    public sealed class WireSphereData : GizmoElement<WireSphere, PrimitiveData>
    {
        public override string MeshPath => "Meshes/WireSphere";
        public override string MaterialPath => "Materials/Common";

        protected override WireSphere CreateInstance()
        {
            WireSphere wireSphere = new WireSphere();
            return wireSphere;
        }
    }

    public sealed unsafe class WireSphere : GizmoRenderer<PrimitiveData>
    {
        public override JobHandle CreateJobHandle()
        {
            var systemBuffer = BatchRendererGroup.GetBuffer();

            fixed (void* buffer = systemBuffer)
            {
                var createJob = new CreatePrimitiveObjectJob()
                {
                    GizmoDataPtr = (PrimitiveData*)JobData.GetUnsafeReadOnlyPtr(),
                    MaxInstanceCount = MaxInstanceCount,
                    Result = buffer
                };

                return createJob.Schedule(RenderCount, 16);
            }
        }
    }
}