using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public unsafe class WireTriangle : PreparingJobScheduler<WireTriangle, PrimitiveData>
    {
        public override JobHandle Schedule()
        {
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 3);

            JobHandle createHandle = new CreateWireTriangleJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = lineBuffer
            }.Schedule(InstanceCount, 16);

            return createHandle;
        }
    }
}