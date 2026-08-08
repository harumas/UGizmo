using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class WireCylinder : PreparingJobScheduler<WireCylinder, CapsuleData>
    {
        private int circleStart;
        private int lineStart;

        public override void ReserveBuffers()
        {
            circleStart = Gizmo<WireCircle, PrimitiveData>.Reserve(InstanceCount * 2);
            lineStart = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);
        }

        public override JobHandle Schedule()
        {
            PrimitiveData* circleBuffer = Gizmo<WireCircle, PrimitiveData>.GetReservedPointer(circleStart);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.GetReservedPointer(lineStart);

            JobHandle jobHandle = new CreateWireCylinderJob()
            {
                GizmoDataPtr = JobDataPtr,
                WireCircleResult = circleBuffer,
                LineResult = lineBuffer
            }.Schedule(InstanceCount, 16);

           return jobHandle;
        }
    }
}