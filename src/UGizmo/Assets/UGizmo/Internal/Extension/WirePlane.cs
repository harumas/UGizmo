using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class WirePlane : PreparingJobScheduler<WirePlane, PrimitiveData>
    {
        public override JobHandle Schedule()
        {
            LineData* planeBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);

            JobHandle jobHandle = new CreateWirePlaneJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = planeBuffer
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}