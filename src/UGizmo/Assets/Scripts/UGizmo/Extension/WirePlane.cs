using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed unsafe class WirePlane : PreparingJobScheduler<WirePlane, PrimitiveData>
    {
        public override JobHandle Schedule()
        {
            LineData* planeBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);

            JobHandle createHandle = new CreateWirePlaneJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = planeBuffer
            }.Schedule(InstanceCount, 16);

            return createHandle;
        }
    }
}