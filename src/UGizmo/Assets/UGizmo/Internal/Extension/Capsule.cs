using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class Capsule : PreparingJobScheduler<Capsule, CapsuleData>
    {
        public override JobHandle Schedule()
        {
            PrimitiveData* hemisphereBuffer = Gizmo<Hemisphere, PrimitiveData>.Reserve(InstanceCount * 2);
            PrimitiveData* tubeBuffer = Gizmo<Tube, PrimitiveData>.Reserve(InstanceCount);

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