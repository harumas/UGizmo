using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class WireCapsule : PreparingJobScheduler<WireCapsule, CapsuleData>
    {
        public override JobHandle Schedule()
        {
            PrimitiveData* hemisphereBuffer = Gizmo<WireHemisphere, PrimitiveData>.Reserve(InstanceCount * 2);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);

            JobHandle jobHandle = new CreateWireCapsuleJob()
            {
                GizmoDataPtr = JobDataPtr,
                HemisphereResult = hemisphereBuffer,
                LineResult = lineBuffer,
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}