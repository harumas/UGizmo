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
    internal unsafe struct CreateCapsuleJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public CapsuleData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public PrimitiveData* HemisphereResult;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public PrimitiveData* TubeResult;

        private static readonly quaternion correctionValue;

        static CreateCapsuleJob()
        {
            correctionValue = quaternion.AxisAngle(new float3(1f, 0f, 0f), math.PI);
        }

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            CapsuleData* capsuleData = GizmoDataPtr + index;

            float radius = capsuleData->Radius;
            float height = math.max(radius * 2f, capsuleData->Height);

            float cos = (1 + math.clamp(capsuleData->UpAxis.y, -1f, 1f)) * 0.5f;
            float3 axis = math.normalizesafe(new float3(capsuleData->UpAxis.z, 0f, -capsuleData->UpAxis.x), new float3(0f, 0f, 1f));

            float4 value = new float4(axis * math.sqrt(1f - cos), math.sqrt(cos));
            quaternion topRotation = new quaternion(value);
            quaternion bottomRotation = math.mul(topRotation, correctionValue);

            float3 scale = new float3(radius, height - radius * 2f, radius);
            TubeResult[index] = new PrimitiveData(capsuleData->Center, topRotation, scale, capsuleData->Color);

            float3 sphereOffset = capsuleData->UpAxis * (height * 0.5f - radius);
            float3 pos1 = capsuleData->Center + sphereOffset;
            float3 pos2 = capsuleData->Center - sphereOffset;

            HemisphereResult[index * 2 + 0] = new PrimitiveData(pos1, topRotation, new float3(radius), capsuleData->Color);
            HemisphereResult[index * 2 + 1] = new PrimitiveData(pos2, bottomRotation, new float3(radius), capsuleData->Color);
        }
    }

    public readonly struct CapsuleData
    {
        public readonly float3 Center;
        public readonly float3 UpAxis;
        public readonly float Height;
        public readonly float Radius;
        public readonly Color Color;

        public CapsuleData(float3 center, float3 upAxis, float height, float radius, Color color)
        {
            Center = center;
            UpAxis = upAxis;
            Height = height;
            Radius = radius;
            Color = color;
        }
    }
}