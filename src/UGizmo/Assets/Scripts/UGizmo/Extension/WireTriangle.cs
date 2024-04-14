using UGizmo.Extension;
using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo
{
    public unsafe class WireTriangle : PreparingJobScheduler<WireTriangle, PrimitiveData>
    {
        public override void Schedule()
        {
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 3);

            JobHandle createHandle = new CreateWireTriangleJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = lineBuffer
            }.Schedule(InstanceCount, 16);

            Gizmo<WireLine, LineData>.AddDependency(createHandle);
        }
    }
}