﻿using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace UGizmo.Extension.Jobs
{
    [BurstCompile]
    public unsafe struct CreateConeJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public ConeData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public RenderData* Result;

        [BurstCompile]
        public void Execute(int index)
        {
            ConeData* coneData = GizmoDataPtr + index;

            float3 position = coneData->Origin + coneData->Direction * coneData->Distance * 0.5f;
            float cos = (1 + math.clamp(-coneData->Direction.y, -1f, 1f)) * 0.5f;
            float3 axis = math.normalizesafe(new float3(-coneData->Direction.z, 0f, coneData->Direction.x), new float3(0f, 0f, 1f));

            float4 value = new float4(axis * math.sqrt(1f - cos), math.sqrt(cos));
            quaternion rotation = new quaternion(value);

            float width = math.tan(coneData->Angle);
            float3 scale = new float3(width, coneData->Distance, width);

            float4x4 matrix = float4x4.TRS(position, rotation, scale);
            Result[index] = new RenderData(matrix, coneData->Color);
        }
    }
}