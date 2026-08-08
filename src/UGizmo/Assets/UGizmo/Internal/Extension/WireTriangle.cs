using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal unsafe class WireTriangle : PreparingJobScheduler<WireTriangle, PrimitiveData>
    {
        private int lineStart;

        public override void ReserveBuffers()
        {
            lineStart = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 3);
        }

        public override JobHandle Schedule()
        {
            LineData* lineBuffer = Gizmo<WireLine, LineData>.GetReservedPointer(lineStart);

            JobHandle jobHandle = new CreateWireTriangleJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = lineBuffer
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}