using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace UGizmo.Extension.Jobs
{
    [BurstCompile]
    public unsafe struct CreateArrowJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public ArrowData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public PrimitiveData* CylinderResult;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public ConeData* ConeResult;

        [BurstCompile]
        public void Execute(int index)
        {
            ArrowData* arrowData = GizmoDataPtr + index;

            float3 diff = arrowData->To - arrowData->From;
            float length = math.length(diff);
            float3 normal = diff * (1f / length);

            float threshold = length / (arrowData->HeadLength * 2f);
            float headLength = threshold <= 1f ? length * 0.5f : arrowData->HeadLength;
            float bodyLength = length - headLength;

            float3 bodyPosition = arrowData->From + normal * bodyLength * 0.5f;

            float cos = (1 + math.clamp(diff.y / length, -1f, 1f)) * 0.5f;
            float3 axis = math.normalizesafe(new float3(diff.z, 0f, -diff.x), new float3(0f, 0f, 1f));

            quaternion rotation = new quaternion(new float4(axis * math.sqrt(1f - cos), math.sqrt(cos)));

            float width = arrowData->Width * math.min(1f, threshold);
            float3 bodyScale = new float3(width, bodyLength, width);

            CylinderResult[index] = new PrimitiveData(bodyPosition, rotation, bodyScale, arrowData->Color);
            ConeResult[index] = new ConeData(arrowData->To, -normal, headLength, math.atan2(width * 0.5f, headLength), arrowData->Color);
        }
    }

    public readonly struct ArrowData
    {
        public readonly float3 From;
        public readonly float3 To;
        public readonly float HeadLength;
        public readonly float Width;
        public readonly Color Color;

        public ArrowData(float3 from, float3 to, Color color, float headLength, float width)
        {
            From = from;
            To = to;
            Color = color;
            HeadLength = headLength;
            Width = width;
        }
    }
}