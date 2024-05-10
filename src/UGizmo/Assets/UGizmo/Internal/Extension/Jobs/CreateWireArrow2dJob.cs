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
    internal unsafe struct CreateWireArrow2dJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public WireArrow2dData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public PrimitiveData* TriangleResult;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public LineData* LineResult;

        private const float FixMultiplier = 1.155f;
        private const int LineCount = 4;

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            WireArrow2dData* arrowData = GizmoDataPtr + index;
            float3 diff = arrowData->To - arrowData->From;
            float length = math.length(diff);
            float3 headNormal = diff * (1f / length);

            float threshold = length / (arrowData->HeadLength * 2f);
            float headLength = threshold <= 1f ? length * 0.5f : arrowData->HeadLength;
            float3 borderPoint = headNormal * (length - headLength);
            float3 right = math.normalize(math.cross(headNormal, arrowData->Normal));
            float3 normal = math.cross(right, headNormal);
            quaternion rotation = quaternion.LookRotation(-normal, headNormal);

            Color planeColor = arrowData->Color;
            planeColor.a = 0.2f;

            float extends = arrowData->HeadWidth * 0.5f;
            float3 p1 = arrowData->From;
            float3 p2 = arrowData->From + borderPoint;
            float3 p3 = arrowData->From + borderPoint + right * extends;
            float3 p4 = arrowData->From + borderPoint + right * -extends;
            float3 p5 = arrowData->To;

            float3 headPosition = (p3 + p4 + p5) / 3f;

            TriangleResult[index] = new PrimitiveData(headPosition, rotation,
                new float3(arrowData->HeadWidth, arrowData->HeadLength * FixMultiplier, 1f),
                planeColor);

            LineResult[LineCount * index + 0] = new LineData(p1, p2, arrowData->Color);
            LineResult[LineCount * index + 1] = new LineData(p3, p4, arrowData->Color);
            LineResult[LineCount * index + 2] = new LineData(p4, p5, arrowData->Color);
            LineResult[LineCount * index + 3] = new LineData(p3, p5, arrowData->Color);
        }
    }

    public readonly struct WireArrow2dData
    {
        public readonly float3 From;
        public readonly float3 To;
        public readonly float3 Normal;
        public readonly Color Color;
        public readonly float HeadLength;
        public readonly float HeadWidth;

        public WireArrow2dData(float3 from, float3 to, float3 normal, Color color, float headLength, float headWidth)
        {
            From = from;
            To = to;
            Normal = normal;
            Color = color;
            HeadLength = headLength;
            HeadWidth = headWidth;
        }
    }
}