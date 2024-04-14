using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed unsafe class WireCylinder : PreparingJobScheduler<WireCylinder, CapsuleData>
    {
        public override void Schedule()
        {
            PrimitiveData* circleBuffer = Gizmo<WireCircle, PrimitiveData>.Reserve(InstanceCount * 2);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);

            JobHandle createHandle = new CreateWireCylinderJob()
            {
                GizmoDataPtr = JobDataPtr,
                WireCircleResult = circleBuffer,
                LineResult = lineBuffer
            }.Schedule(InstanceCount, 16);

            Gizmo<WireLine, LineData>.AddDependency(createHandle);
            Gizmo<WireCircle, PrimitiveData>.AddDependency(createHandle);
        }
    }
}