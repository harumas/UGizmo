using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class Capsule : PreparingJobScheduler<Capsule, CapsuleData>
    {
        private int hemisphereStart;
        private int tubeStart;

        public override void ReserveBuffers()
        {
            hemisphereStart = Gizmo<Hemisphere, PrimitiveData>.Reserve(InstanceCount * 2);
            tubeStart = Gizmo<Tube, PrimitiveData>.Reserve(InstanceCount);
        }

        public override JobHandle Schedule()
        {
            PrimitiveData* hemisphereBuffer = Gizmo<Hemisphere, PrimitiveData>.GetReservedPointer(hemisphereStart);
            PrimitiveData* tubeBuffer = Gizmo<Tube, PrimitiveData>.GetReservedPointer(tubeStart);

            JobHandle jobHandle = new CreateCapsuleJob()
            {
                GizmoDataPtr = JobDataPtr,
                HemisphereResult = hemisphereBuffer,
                TubeResult = tubeBuffer,
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}