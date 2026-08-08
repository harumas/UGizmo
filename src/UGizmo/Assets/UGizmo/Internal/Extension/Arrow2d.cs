using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal unsafe class Arrow2d : PreparingJobScheduler<Arrow2d, Arrow2dData>
    {
        private int planeStart;
        private int triangleStart;
        private int lineStart;

        public override void ReserveBuffers()
        {
            planeStart = Gizmo<Plane, PrimitiveData>.Reserve(InstanceCount);
            triangleStart = Gizmo<Triangle, PrimitiveData>.Reserve(InstanceCount);
            lineStart = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 7);
        }

        public override JobHandle Schedule()
        {
            PrimitiveData* planeBuffer = Gizmo<Plane, PrimitiveData>.GetReservedPointer(planeStart);
            PrimitiveData* triangleBuffer = Gizmo<Triangle, PrimitiveData>.GetReservedPointer(triangleStart);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.GetReservedPointer(lineStart);

            JobHandle jobHandle = new CreateArrow2dJob()
            {
                GizmoDataPtr = JobDataPtr,
                PlaneResult = planeBuffer,
                TriangleResult = triangleBuffer,
                LineResult = lineBuffer
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}