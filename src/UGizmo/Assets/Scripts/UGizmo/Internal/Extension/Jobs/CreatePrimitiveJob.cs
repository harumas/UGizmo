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
        public RenderData* Result;

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            PrimitiveData* renderData = GizmoDataPtr + index;

            float4x4 matrix = float4x4.TRS(renderData->Position, renderData->Rotation, renderData->Scale);

            Result[index] = new RenderData(matrix, renderData->Color);
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