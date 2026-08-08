using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class Capsule2d : PreparingJobScheduler<Capsule2d, Capsule2dData>
    {
        private int semicircleStart;
        private int planeStart;

        public override void ReserveBuffers()
        {
            semicircleStart = Gizmo<Semicircle, PrimitiveData>.Reserve(InstanceCount * 2);
            planeStart = Gizmo<Plane, PrimitiveData>.Reserve(InstanceCount);
        }

        public override JobHandle Schedule()
        {
            PrimitiveData* semicircleBuffer = Gizmo<Semicircle, PrimitiveData>.GetReservedPointer(semicircleStart);
            PrimitiveData* planeBuffer = Gizmo<Plane, PrimitiveData>.GetReservedPointer(planeStart);

            JobHandle jobHandle = new CreateCapsule2dJob()
            {
                GizmoDataPtr = JobDataPtr,
                SemicircleResult = semicircleBuffer,
                PlaneResult = planeBuffer,
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }

}