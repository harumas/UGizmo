using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class WireCylinder : PreparingJobScheduler<WireCylinder, CapsuleData>
    {
        public override JobHandle Schedule()
        {
            PrimitiveData* circleBuffer = Gizmo<WireCircle, PrimitiveData>.Reserve(InstanceCount * 2);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);

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