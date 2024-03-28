using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UGizmo.Extension.Jobs
{
    [BurstCompile]
    internal unsafe struct CreateWireLineJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public LineData* GizmoDataPtr;

        public int MaxInstanceCount;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public RenderData* Result;

        private static readonly int stride = UnsafeUtility.SizeOf<float4>();

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            LineData* renderData = GizmoDataPtr + index;

            float3 diff = renderData->End - renderData->Start;
            float3 position = (float3)renderData->Start + diff * 0.5f;
            float lengthE = math.length(diff);

            float cos = (1 + math.clamp(diff.z / lengthE, -1f, 1f)) * 0.5f;
            float3 axis = math.normalize(new float3(-diff.y, diff.x, 0f));
            quaternion rotation = new quaternion(new float4(axis * math.sqrt(1 - cos), math.sqrt(cos)));

            float4x4 matrix = float4x4.TRS(position, rotation, new float3(0f, 0f, lengthE));

            Result[index] = new RenderData(matrix, renderData->Color);
        }
    }
}