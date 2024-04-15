using UGizmo.Extension.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UGizmo.Extension
{
    public class GizmoJobBuffer<TJobData> where TJobData : unmanaged
    {
        private NativeList<TJobData> jobData = new NativeList<TJobData>(Allocator.Persistent);

        public NativeSlice<TJobData> Reserve(int instanceCount)
        {
            int start = jobData.Length;
            jobData.AddReplicate(default, instanceCount);
            return jobData.AsArray().Slice(start, instanceCount);
        }

        public JobHandle CreateJobHandle()
        {
            var createJob = new CreatePrimitiveJob()
            {
                GizmoDataPtr = jobData.AsParallelReader(),
                Result = RenderBufferPtr
            };

            return createJob.Schedule(InstanceCount, 16, Dependency);
        }
    }
}