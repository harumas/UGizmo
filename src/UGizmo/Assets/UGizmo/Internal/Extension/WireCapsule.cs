using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class WireCapsule : PreparingJobScheduler<WireCapsule, CapsuleData>
    {
        private int hemisphereStart;
        private int lineStart;

        public override void ReserveBuffers()
        {
            hemisphereStart = Gizmo<WireHemisphere, PrimitiveData>.Reserve(InstanceCount * 2);
            lineStart = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);
        }

        public override JobHandle Schedule()
        {
            PrimitiveData* hemisphereBuffer = Gizmo<WireHemisphere, PrimitiveData>.GetReservedPointer(hemisphereStart);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.GetReservedPointer(lineStart);

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