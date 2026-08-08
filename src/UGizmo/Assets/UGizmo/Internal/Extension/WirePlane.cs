using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class WirePlane : PreparingJobScheduler<WirePlane, PrimitiveData>
    {
        private int planeStart;

        public override void ReserveBuffers()
        {
            planeStart = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);
        }

        public override JobHandle Schedule()
        {
            LineData* planeBuffer = Gizmo<WireLine, LineData>.GetReservedPointer(planeStart);

            JobHandle jobHandle = new CreateWirePlaneJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = planeBuffer
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}