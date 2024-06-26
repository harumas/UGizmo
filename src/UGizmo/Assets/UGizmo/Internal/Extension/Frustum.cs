﻿using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal sealed unsafe class Frustum : PreparingJobScheduler<Frustum, FrustumData>
    {
        private const int LineCount = 12;

        public override JobHandle Schedule()
        {
            var buffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * LineCount);

            JobHandle jobHandle = new CreateFrustumJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = buffer
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}