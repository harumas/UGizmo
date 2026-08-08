using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class Arrow : PreparingJobScheduler<Arrow, ArrowData>
    {
        private int cylinderStart;
        private int coneStart;

        public override void ReserveBuffers()
        {
            cylinderStart = Gizmo<Cylinder, PrimitiveData>.Reserve(InstanceCount);
            coneStart = Gizmo<Cone, ConeData>.Reserve(InstanceCount);
        }

        public override JobHandle Schedule()
        {
            PrimitiveData* cylinderBuffer = Gizmo<Cylinder, PrimitiveData>.GetReservedPointer(cylinderStart);
            ConeData* coneBuffer = Gizmo<Cone, ConeData>.GetReservedPointer(coneStart);

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