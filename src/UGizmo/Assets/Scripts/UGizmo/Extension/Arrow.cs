using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed unsafe class Arrow : PreparingJobScheduler<Arrow, ArrowData>
    {
        public override void Schedule()
        {
            PrimitiveData* cylinderBuffer = Gizmo<Cylinder, PrimitiveData>.Reserve(InstanceCount);
            ConeData* coneBuffer = Gizmo<Cone, ConeData>.Reserve(InstanceCount);

            JobHandle jobHandle = new CreateArrowJob()
            {
                GizmoDataPtr = JobDataPtr,
                CylinderResult = cylinderBuffer,
                ConeResult = coneBuffer,
            }.Schedule(InstanceCount, 16);

            Gizmo<Cylinder, PrimitiveData>.AddDependency(jobHandle);
            Gizmo<Cone, ConeData>.AddDependency(jobHandle);
        }
    }
}