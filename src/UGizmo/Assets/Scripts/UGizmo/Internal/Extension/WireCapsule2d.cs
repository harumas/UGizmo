using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class WireCapsule2d : PreparingJobScheduler<WireCapsule2d, Capsule2dData>
    {
        public override JobHandle Schedule()
        {
            PrimitiveData* semicircleBuffer = Gizmo<WireSemicircle, PrimitiveData>.Reserve(InstanceCount * 2);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 2);
            
            JobHandle jobHandle = new CreateWireCapsule2dJob()
            {
                GizmoDataPtr = JobDataPtr,
                WireSemicircleResult = semicircleBuffer,
                LineResult = lineBuffer,
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}