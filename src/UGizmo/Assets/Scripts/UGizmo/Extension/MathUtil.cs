using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace UGizmo.Extension
{
    [BurstCompile]
    public static class MathUtil
    {
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pack3x4(in float4x4 matrix, out float3x4 result)
        {
            result = new float3x4(
                new float3(matrix.c0.x, matrix.c0.y, matrix.c0.z),
                new float3(matrix.c1.x, matrix.c1.y, matrix.c1.z),
                new float3(matrix.c2.x, matrix.c2.y, matrix.c2.z),
                new float3(matrix.c3.x, matrix.c3.y, matrix.c3.z));
        }
    }
}