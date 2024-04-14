using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed unsafe class WireCapsule : PreparingJobScheduler<WireCapsule, CapsuleData>
    {
        public override void Schedule()
        {
            PrimitiveData* hemisphereBuffer = Gizmo<WireHemisphere, PrimitiveData>.Reserve(InstanceCount * 2);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);

            var createJob = new CreateWireCapsuleJob()
            {
                GizmoDataPtr = JobDataPtr,
                HemisphereResult = hemisphereBuffer,
                LineResult = lineBuffer,
            }.Schedule(InstanceCount, 16);

            Gizmo<Hemisphere, PrimitiveData>.AddDependency(createJob);
            Gizmo<WireLine, LineData>.AddDependency(createJob);
        }
    }
}