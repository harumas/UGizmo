using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace UGizmo.Extension.Jobs
{
    [BurstCompile]
    public unsafe struct CreateWireConeJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public ConeData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public PrimitiveData* WireCircleResult;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public LineData* LineResult;

        private static readonly float3 front = new float3(0f, 0f, 1f);
        private static readonly float3 right = new float3(1f, 0f, 0f);
        private const int LineCount = 4;


        [BurstCompile]
        public void Execute(int index)
        {
            ConeData* coneData = GizmoDataPtr + index;

            float3 position = coneData->Origin + coneData->Direction * coneData->Distance;
            float cos = (1 + math.clamp(coneData->Direction.y, -1f, 1f)) * 0.5f;
            float3 axis = math.normalizesafe(new float3(coneData->Direction.z, 0f, -coneData->Direction.x), new float3(0f, 0f, 1f));

            float4 value = new float4(axis * math.sqrt(1f - cos), math.sqrt(cos));
            quaternion rotation = new quaternion(value);
            float width = math.tan(coneData->Angle);

            WireCircleResult[index] = new PrimitiveData(position, rotation, new float3(width, 0f, width), coneData->Color);

            float3 rotateFront = math.rotate(rotation, front * width);
            float3 rotateRight = math.rotate(rotation, right * width);

            float3 p1 = coneData->Origin;
            float3 p2 = rotateFront + position;
            LineResult[LineCount * index + 0] = new LineData(p1, p2, coneData->Color);

            float3 p3 = coneData->Origin;
            float3 p4 = -rotateFront + position;
            LineResult[LineCount * index + 1] = new LineData(p3, p4, coneData->Color);

            float3 p5 = coneData->Origin;
            float3 p6 = rotateRight + position;
            LineResult[LineCount * index + 2] = new LineData(p5, p6, coneData->Color);

            float3 p7 = coneData->Origin;
            float3 p8 = -rotateRight + position;
            LineResult[LineCount * index + 3] = new LineData(p7, p8, coneData->Color);
        }
    }

    public readonly struct ConeData
    {
        public readonly float3 Origin;
        public readonly float3 Direction;
        public readonly float Distance;
        public readonly float Angle;
        public readonly Color Color;

        public ConeData(float3 origin, float3 direction, float distance, float angle, Color color)
        {
            Origin = origin;
            Direction = direction;
            Distance = distance;
            Angle = angle;
            Color = color;
        }
    }
}