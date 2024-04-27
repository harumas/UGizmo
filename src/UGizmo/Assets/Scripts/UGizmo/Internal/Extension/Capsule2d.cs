using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class Capsule2d : PreparingJobScheduler<Capsule2d, Capsule2dData>
    {
        public override JobHandle Schedule()
        {
            PrimitiveData* semicircleBuffer = Gizmo<Semicircle, PrimitiveData>.Reserve(InstanceCount * 2);
            PrimitiveData* planeBuffer = Gizmo<Plane, PrimitiveData>.Reserve(InstanceCount);

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