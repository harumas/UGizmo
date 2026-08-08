using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class WireCapsule2d : PreparingJobScheduler<WireCapsule2d, Capsule2dData>
    {
        private int semicircleStart;
        private int lineStart;

        public override void ReserveBuffers()
        {
            semicircleStart = Gizmo<WireSemicircle, PrimitiveData>.Reserve(InstanceCount * 2);
            lineStart = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 2);
        }

        public override JobHandle Schedule()
        {
            PrimitiveData* semicircleBuffer = Gizmo<WireSemicircle, PrimitiveData>.GetReservedPointer(semicircleStart);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.GetReservedPointer(lineStart);

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