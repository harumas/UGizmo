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
    internal unsafe struct CreateWireSector2dJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public Sector2dData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public LineData* LineResult;

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            Sector2dData* data = GizmoDataPtr + index;
            
            int segments = data->Segments;
            float angleStep = data->AngleRad / segments;
            float startAngle = -data->AngleRad * 0.5f;

            float3 prevPoint = default;
            float3 up = new float3(0, 1, 0);

            int startIdx = data->StartIndex;

            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = startAngle + angleStep * i;
                quaternion localRot = quaternion.AxisAngle(new float3(0, 0, 1), currentAngle);
                quaternion drawRot = math.mul(data->Rotation, localRot);
                
                float3 dir = math.mul(drawRot, up);
                float3 point = data->Center + dir * data->Radius;
                
                if (i > 0)
                {
                    LineResult[startIdx + (i - 1)] = new LineData(prevPoint, point, data->Color);
                }
                
                if (i == 0)
                {
                    LineResult[startIdx + segments] = new LineData(data->Center, point, data->Color);
                }
                if (i == segments)
                {
                    LineResult[startIdx + segments + 1] = new LineData(data->Center, point, data->Color);
                }
                
                prevPoint = point;
            }
        }
    }
}
