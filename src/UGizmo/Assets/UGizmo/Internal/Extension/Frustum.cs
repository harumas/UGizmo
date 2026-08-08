using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class Frustum : PreparingJobScheduler<Frustum, FrustumData>
    {
        private const int LineCount = 12;

        private int lineStart;

        public override void ReserveBuffers()
        {
            lineStart = Gizmo<WireLine, LineData>.Reserve(InstanceCount * LineCount);
        }

        public override JobHandle Schedule()
        {
            LineData* buffer = Gizmo<WireLine, LineData>.GetReservedPointer(lineStart);

            JobHandle jobHandle = new CreateFrustumJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = buffer
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}