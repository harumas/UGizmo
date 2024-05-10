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

            float3 diff = data->End - data->Start;
            float3 position = data->Start + diff * 0.5f;
            float length = math.length(diff);

            float cos = (1 + math.clamp(diff.z / length, -1f, 1f)) * 0.5f;
            float3 axis = math.normalizesafe(new float3(-diff.y, diff.x, 0f), new float3(0f, 0f, 1f));
            quaternion rotation = new quaternion(new float4(axis * math.sqrt(1 - cos), math.sqrt(cos)));
            float4x4 matrix = float4x4.TRS(position, rotation, new float3(0f, 0f, length));

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