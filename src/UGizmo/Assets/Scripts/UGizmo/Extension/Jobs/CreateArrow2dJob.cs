using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace UGizmo.Extension.Jobs
{
    [BurstCompile]
    public unsafe struct CreateArrow2dJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public Arrow2dData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public PrimitiveData* PlaneResult;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public PrimitiveData* TriangleResult;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public LineData* LineResult;

        private const float HeadMultiplier = 2f;
        private const float FixMultiplier = 1.155f;
        private const int LineCount = 7;

        [BurstCompile]
        public void Execute(int index)
        {
            Arrow2dData* arrowData = GizmoDataPtr + index;
            float3 diff = arrowData->To - arrowData->From;
            float length = math.length(diff);
            float3 headNormal = diff * (1f / length);

            float threshold = length / (arrowData->HeadLength * 2f);
            float headLength = threshold <= 1f ? length * 0.5f : arrowData->HeadLength;
            float bodyLength = length - headLength;
            float3 borderPoint = headNormal * bodyLength;
            float3 right = math.normalize(math.cross(headNormal, arrowData->Normal));
            float3 normal = math.cross(right, headNormal);
            quaternion rotation = quaternion.LookRotation(-normal, headNormal);
            quaternion rotationPlane = quaternion.LookRotation(-headNormal, normal);

            Color planeColor = arrowData->Color;
            planeColor.a = 0.2f;

            float extends = arrowData->Width * 0.5f;
            float3 p1 = arrowData->From + right * extends;
            float3 p2 = arrowData->From + right * -extends;
            float3 p3 = p1 + borderPoint;
            float3 p4 = p2 + borderPoint;
            float3 p5 = arrowData->From + borderPoint + right * (extends * HeadMultiplier);
            float3 p6 = arrowData->From + borderPoint + right * -(extends * HeadMultiplier);
            float3 p7 = arrowData->To;

            float3 bodyPosition = arrowData->From + borderPoint * 0.5f;
            float3 headPosition = (p5 + p6 + p7) / 3f;


            PlaneResult[index] = new PrimitiveData(bodyPosition, rotationPlane, new float3(arrowData->Width, 1f, bodyLength), planeColor);
            TriangleResult[index] = new PrimitiveData(headPosition, rotation,
                new float3(arrowData->Width * HeadMultiplier, headLength * FixMultiplier, 1f),
                planeColor);

            LineResult[LineCount * index + 0] = new LineData(p1, p2, arrowData->Color);
            LineResult[LineCount * index + 1] = new LineData(p1, p3, arrowData->Color);
            LineResult[LineCount * index + 2] = new LineData(p2, p4, arrowData->Color);
            LineResult[LineCount * index + 3] = new LineData(p3, p5, arrowData->Color);
            LineResult[LineCount * index + 4] = new LineData(p4, p6, arrowData->Color);
            LineResult[LineCount * index + 5] = new LineData(p5, p7, arrowData->Color);
            LineResult[LineCount * index + 6] = new LineData(p6, p7, arrowData->Color);
        }
    }

    public readonly struct Arrow2dData
    {
        public readonly float3 From;
        public readonly float3 To;
        public readonly float3 Normal;
        public readonly Color Color;
        public readonly float HeadLength;
        public readonly float Width;

        public Arrow2dData(float3 from, float3 to, float3 normal, Color color, float headLength, float width)
        {
            From = from;
            To = to;
            Normal = normal;
            Color = color;
            HeadLength = headLength;
            Width = width;
        }
    }
}