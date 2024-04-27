using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public unsafe class WireArrow2d : PreparingJobScheduler<WireArrow2d, WireArrow2dData>
    {
        public override JobHandle Schedule()
        {
            PrimitiveData* triangleBuffer = Gizmo<Triangle, PrimitiveData>.Reserve(InstanceCount);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);

            JobHandle createJobHandle = new CreateWireArrow2dJob()
            {
                GizmoDataPtr = JobDataPtr,
                TriangleResult = triangleBuffer,
                LineResult = lineBuffer
            }.Schedule(InstanceCount, 16);

            return createJobHandle;
        }
    }
}