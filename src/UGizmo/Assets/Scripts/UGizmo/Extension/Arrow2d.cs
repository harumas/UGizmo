using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public unsafe class Arrow2d : PreparingJobScheduler<Arrow2d, Arrow2dData>
    {
        public override JobHandle Schedule()
        {
            PrimitiveData* planeBuffer = Gizmo<Plane, PrimitiveData>.Reserve(InstanceCount);
            PrimitiveData* triangleBuffer = Gizmo<Triangle, PrimitiveData>.Reserve(InstanceCount);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 7);

            JobHandle createHandle = new CreateArrow2dJob()
            {
                GizmoDataPtr = JobDataPtr,
                PlaneResult = planeBuffer,
                TriangleResult = triangleBuffer,
                LineResult = lineBuffer
            }.Schedule(InstanceCount, 16);

            return createHandle;
        }
    }
}