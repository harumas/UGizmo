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
    internal unsafe struct CreateCapsule2dJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public Capsule2dData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public PrimitiveData* SemicircleResult;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public PrimitiveData* PlaneResult;

        private static readonly quaternion correctionValue;
        private static readonly quaternion quadRotation;

        static CreateCapsule2dJob()
        {
            correctionValue = quaternion.AxisAngle(new float3(1f, 0f, 0f), math.PI);
            quadRotation = quaternion.Euler(math.PI / 2f, 0f, 0f);
        }

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            Capsule2dData* capsuleData = GizmoDataPtr + index;

            float radius = capsuleData->Radius;
            float height = math.max(radius * 2f, capsuleData->Height);

            quaternion bottomRotation = math.mul(capsuleData->Rotation, correctionValue);
            quaternion planeRotation = math.mul(capsuleData->Rotation, quadRotation);

            //Body Quad
            float3 scale = new float3(radius * 2f, 1f, height - radius * 2f);
            PlaneResult[index] = new PrimitiveData(capsuleData->Center, planeRotation, scale, capsuleData->Color);

            //Top and Bottom
            float3 headNormal = math.rotate(capsuleData->Rotation, new float3(0f, 1f, 0f));
            float3 sphereOffset = headNormal * (height * 0.5f - radius);
            float3 pos1 = capsuleData->Center + sphereOffset;
            float3 pos2 = capsuleData->Center - sphereOffset;

            SemicircleResult[index * 2 + 0] = new PrimitiveData(pos1, capsuleData->Rotation, new float3(radius), capsuleData->Color);
            SemicircleResult[index * 2 + 1] = new PrimitiveData(pos2, bottomRotation, new float3(radius), capsuleData->Color);
        }
    }

    public readonly struct Capsule2dData
    {
        public readonly float3 Center;
        public readonly quaternion Rotation;
        public readonly float Height;
        public readonly float Radius;
        public readonly Color Color;

        public Capsule2dData(float3 center, quaternion rotation, float height, float radius, Color color)
        {
            Center = center;
            Rotation = rotation;
            Height = height;
            Radius = radius;
            Color = color;
        }
    }
}