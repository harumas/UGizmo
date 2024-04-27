using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed unsafe class WireCone : PreparingJobScheduler<WireCone, ConeData>
    {
        public override JobHandle Schedule()
        {
            PrimitiveData* circleBuffer = Gizmo<WireCircle, PrimitiveData>.Reserve(InstanceCount);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);

            JobHandle createHandle = new CreateWireConeJob()
            {
                GizmoDataPtr = JobDataPtr,
                WireCircleResult = circleBuffer,
                LineResult = lineBuffer
            }.Schedule(InstanceCount, 16);

            return createHandle;
        }
    }
}