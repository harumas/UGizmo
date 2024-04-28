using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class Arrow : PreparingJobScheduler<Arrow, ArrowData>
    {
        public override JobHandle Schedule()
        {
            PrimitiveData* cylinderBuffer = Gizmo<Cylinder, PrimitiveData>.Reserve(InstanceCount);
            ConeData* coneBuffer = Gizmo<Cone, ConeData>.Reserve(InstanceCount);

            JobHandle jobHandle = new CreateArrowJob()
            {
                GizmoDataPtr = JobDataPtr,
                CylinderResult = cylinderBuffer,
                ConeResult = coneBuffer,
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}