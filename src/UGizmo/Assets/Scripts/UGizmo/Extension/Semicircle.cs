﻿using UGizmo.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public sealed class SemicircleAsset : GizmoAsset<Semicircle, PrimitiveData>
    {
        public override string MeshName => "Semicircle";
        public override string MaterialName => "CommonMeshCullOff";
    }

    public sealed unsafe class Semicircle : GizmoRenderer<PrimitiveData>
    {
        public override JobHandle CreateJobHandle()
        {
            var createJob = new CreatePrimitiveJob()
            {
                GizmoDataPtr = JobDataPtr,
                Result = RenderBufferPtr
            };

            return createJob.Schedule(InstanceCount, 16, Dependency);
        }
    }
}