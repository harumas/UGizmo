using UGizmo.Extension.Jobs;
using Unity.Jobs;
using UnityEngine;

namespace UGizmo.Extension
{
    public sealed unsafe class Frustum : PreparingJobScheduler<Frustum, FrustumData>
    {
        private const int LineCount = 12;

        public override void Schedule()
        {
            var buffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * LineCount);

            JobHandle jobHandle = new CreateFrustumJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = buffer
            }.Schedule(InstanceCount, 16);

            Gizmo<WireLine, LineData>.AddDependency(jobHandle);
        }
    }
}