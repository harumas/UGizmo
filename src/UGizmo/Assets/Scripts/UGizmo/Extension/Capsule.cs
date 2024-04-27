using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed unsafe class Capsule : PreparingJobScheduler<Capsule, CapsuleData>
    {
        public override JobHandle Schedule()
        {
            PrimitiveData* hemisphereBuffer = Gizmo<Hemisphere, PrimitiveData>.Reserve(InstanceCount * 2);
            PrimitiveData* tubeBuffer = Gizmo<Tube, PrimitiveData>.Reserve(InstanceCount);

            JobHandle createJob = new CreateCapsuleJob()
            {
                GizmoDataPtr = JobDataPtr,
                HemisphereResult = hemisphereBuffer,
                TubeResult = tubeBuffer,
            }.Schedule(InstanceCount, 16);

            return createJob;
        }
    }
}