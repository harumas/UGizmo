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
    internal unsafe struct CreatePrimitiveJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public PrimitiveData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public DrawData* Result;

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            PrimitiveData* drawData = GizmoDataPtr + index;

            float4x4 matrix = float4x4.TRS(drawData->Position, drawData->Rotation, drawData->Scale);

            Result[index] = new DrawData(matrix, drawData->Color);
        }
    }

    public readonly struct PrimitiveData
    {
        public readonly float3 Position;
        public readonly quaternion Rotation;
        public readonly float3 Scale;
        public readonly Color Color;

        public PrimitiveData(float3 position, quaternion rotation, float3 scale, Color color)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Color = color;
        }
    }
}