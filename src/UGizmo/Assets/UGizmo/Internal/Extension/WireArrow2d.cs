using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class WireArrow2d : PreparingJobScheduler<WireArrow2d, WireArrow2dData>
    {
        private int triangleStart;
        private int lineStart;

        public override void ReserveBuffers()
        {
            triangleStart = Gizmo<Triangle, PrimitiveData>.Reserve(InstanceCount);
            lineStart = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);
        }

        public override JobHandle Schedule()
        {
            PrimitiveData* triangleBuffer = Gizmo<Triangle, PrimitiveData>.GetReservedPointer(triangleStart);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.GetReservedPointer(lineStart);

            JobHandle jobHandle = new CreateWireArrow2dJob()
            {
                GizmoDataPtr = JobDataPtr,
                TriangleResult = triangleBuffer,
                LineResult = lineBuffer
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}