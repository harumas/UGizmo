using System.Runtime.CompilerServices;
using UGizmos.MeshGenerator;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UGizmos
{
    [BurstCompile]
    internal unsafe struct CreateRenderDataJob<T> : IJobParallelFor where T : unmanaged
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public GizmoData<T>* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public void* Result;

        public int MaxInstanceCount;

        private static readonly int stride = UnsafeUtility.SizeOf<float4>();
        private static readonly int customDataSize = UnsafeUtility.SizeOf<T>() == 1 ? 0 : UnsafeUtility.SizeOf<T>() / stride;

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            GizmoData<T>* renderData = GizmoDataPtr + index;

            float4x4 matrix = float4x4.TRS(renderData->Position, renderData->Rotation, renderData->Scale);
            float4x4 inversedMatrix = math.inverse(matrix);

            UnsafeUtility.WriteArrayElementWithStride(Result, index * 3, stride, Pack(matrix));
            UnsafeUtility.WriteArrayElementWithStride(Result, MaxInstanceCount * 3 + index * 3, stride, Pack(inversedMatrix));
            UnsafeUtility.WriteArrayElementWithStride(Result, MaxInstanceCount * 6 + index * 1, stride, renderData->Color);

            if (customDataSize > 0)
            {
                UnsafeUtility.WriteArrayElementWithStride(Result, MaxInstanceCount * 7 + index * customDataSize, stride, renderData->CustomData);
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float3x4 Pack(in float4x4 matrix)
        {
            return new float3x4(
                new float3(matrix.c0.x, matrix.c0.y, matrix.c0.z),
                new float3(matrix.c1.x, matrix.c1.y, matrix.c1.z),
                new float3(matrix.c2.x, matrix.c2.y, matrix.c2.z),
                new float3(matrix.c3.x, matrix.c3.y, matrix.c3.z));
        }
    }
}