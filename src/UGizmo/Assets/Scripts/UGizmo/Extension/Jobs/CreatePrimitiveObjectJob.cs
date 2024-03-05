using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UGizmo.Extension.Jobs
{
    [BurstCompile]
    internal unsafe struct CreatePrimitiveObjectJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public PrimitiveData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public void* Result;

        public int MaxInstanceCount;

        private static readonly int stride = UnsafeUtility.SizeOf<float4>();

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            PrimitiveData* renderData = GizmoDataPtr + index;

            float4x4 matrix = float4x4.TRS(renderData->Position, renderData->Rotation, renderData->Scale);
            MathUtil.Pack3x4(matrix, out float3x4 packedMatrix);
            MathUtil.Pack3x4(math.fastinverse(matrix), out float3x4 inversedMatrix);

            UnsafeUtility.WriteArrayElementWithStride(Result, index * 3, stride, packedMatrix);
            UnsafeUtility.WriteArrayElementWithStride(Result, MaxInstanceCount * 3 + index * 3, stride, inversedMatrix);
            UnsafeUtility.WriteArrayElementWithStride(Result, MaxInstanceCount * 6 + index * 1, stride, renderData->Color);
        }
    }
}