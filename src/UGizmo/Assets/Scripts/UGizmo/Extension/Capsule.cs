using UGizmo.Extension;
using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo
{
    public sealed unsafe class Capsule : PreparingJobScheduler<Capsule, CapsuleData>
    {
        public override void Schedule()
        {
            PrimitiveData* hemisphereBuffer = Gizmo<Hemisphere, PrimitiveData>.Reserve(InstanceCount * 2);
            PrimitiveData* tubeBuffer = Gizmo<Tube, PrimitiveData>.Reserve(InstanceCount);

            var createJob = new CreateCapsuleJob()
            {
                GizmoDataPtr = JobDataPtr,
                HemisphereResult = hemisphereBuffer,
                TubeResult = tubeBuffer,
            }.Schedule(InstanceCount, 16);

            Gizmo<Hemisphere, PrimitiveData>.AddDependency(createJob);
            Gizmo<Tube, PrimitiveData>.AddDependency(createJob);
        }
    }
}