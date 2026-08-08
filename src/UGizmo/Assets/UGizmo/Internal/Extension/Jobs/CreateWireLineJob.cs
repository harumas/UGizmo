using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace UGizmo.Internal.Extension.Jobs
{
    [BurstCompile]
    internal unsafe struct CreateWireLineJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public LineData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public DrawData* Result;

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            LineData* data = GizmoDataPtr + index;

            float3 start = data->Start;
            float3 diff = data->End - start;

            // The line mesh runs along Z and is scaled by (0, 0, length), so the first two columns
            // of the rotation are multiplied away and only its third column survives - as
            // normalize(diff) * length, which is just diff. Building a quaternion from the
            // direction and letting TRS convert it back into a matrix computed exactly this, at the
            // cost of a length, a normalize, two square roots and a quaternion-to-matrix conversion
            // per line. Lines are the most numerous gizmo (every wire capsule is four of them, a
            // frustum twelve), so this ran a lot.
            // Going straight to the matrix is also better behaved for a zero-length line: the old
            // form divided by length and produced NaNs, this one collapses to a point.
            float4x4 matrix = new float4x4(
                float4.zero,
                float4.zero,
                new float4(diff, 0f),
                new float4(start + diff * 0.5f, 1f));

            Result[index] = new DrawData(matrix, data->Color);
        }
    }

    public struct LineData
    {
        public readonly float3 Start;
        public readonly float3 End;
        public readonly Color Color;

        public LineData(float3 start, float3 end, Color color)
        {
            Start = start;
            End = end;
            Color = color;
        }
    }
}