using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace UGizmo.Internal.Extension.Jobs
{
    public struct Sector2dData
    {
        public readonly float3 Center;
        public readonly quaternion Rotation;
        public readonly float Radius;
        public readonly float AngleRad;
        public readonly Color Color;
        public readonly int Segments;
        public int StartIndex;

        public Sector2dData(float3 center, quaternion rotation, float radius, float angleRad, Color color, int segments)
        {
            Center = center;
            Rotation = rotation;
            Radius = radius;
            AngleRad = angleRad;
            Color = color;
            Segments = segments;
            StartIndex = 0;
        }
    }

    [BurstCompile]
    internal unsafe struct CreateSector2dJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public Sector2dData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public PrimitiveData* TriangleResult;

        private const float FixMultiplier = 1.15470054f; // 2 / sqrt(3) is needed to normalize the height of an equilateral triangle

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            Sector2dData* data = GizmoDataPtr + index;
            
            float angleStep = data->AngleRad / data->Segments;
            float startAngle = -data->AngleRad * 0.5f;
            
            float halfWidth = data->Radius * math.sin(angleStep * 0.5f);
            float height = data->Radius * math.cos(angleStep * 0.5f);
            float3 scale = new float3(halfWidth * 2f, height * FixMultiplier, 1f);

            int startIdx = data->StartIndex;

            for (int i = 0; i < data->Segments; i++)
            {
                float currentAngle = startAngle + angleStep * (i + 0.5f);
                
                quaternion localRot = quaternion.AxisAngle(new float3(0, 0, 1), currentAngle);
                quaternion drawRot = math.mul(data->Rotation, localRot);
                
                // Direction from Center to arc (Outward)
                float3 dir = math.mul(drawRot, new float3(0, 1, 0));
                
                quaternion triangleRot = math.mul(drawRot, quaternion.Euler(0, 0, math.PI));
                
                float3 drawPos = data->Center + dir * (height * 2f / 3f);
                
                TriangleResult[startIdx + i] = new PrimitiveData(drawPos, triangleRot, scale, data->Color);
            }
        }
    }
}
