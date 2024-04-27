using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed unsafe class WireCapsule2d : PreparingJobScheduler<WireCapsule2d, Capsule2dData>
    {
        public override JobHandle Schedule()
        {
            PrimitiveData* semicircleBuffer = Gizmo<WireSemicircle, PrimitiveData>.Reserve(InstanceCount * 2);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 2);
            
            JobHandle createJob = new CreateWireCapsule2dJob()
            {
                GizmoDataPtr = JobDataPtr,
                WireSemicircleResult = semicircleBuffer,
                LineResult = lineBuffer,
            }.Schedule(InstanceCount, 16);

            return createJob;
        }
    }
}