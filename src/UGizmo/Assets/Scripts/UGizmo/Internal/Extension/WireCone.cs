using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class WireCone : PreparingJobScheduler<WireCone, ConeData>
    {
        public override JobHandle Schedule()
        {
            PrimitiveData* circleBuffer = Gizmo<WireCircle, PrimitiveData>.Reserve(InstanceCount);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);

            JobHandle jobHandle = new CreateWireConeJob()
            {
                GizmoDataPtr = JobDataPtr,
                WireCircleResult = circleBuffer,
                LineResult = lineBuffer
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}