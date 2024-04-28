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
    internal unsafe struct CreateFrustumJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public FrustumData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public LineData* Result;

        private const int LineCount = 12;

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            var data = GizmoDataPtr + index;

            float near = data->NearClipPlane;
            float far = data->FarClipPlane;
            float fov = data->Fov;
            float aspect = data->Aspect;
            float height = math.tan(math.radians(fov * 0.5f));

            float heightNear = height * near;
            float widthNear = heightNear * aspect;

            float heightFar = height * far;
            float widthFar = heightFar * aspect;

            float3 center = data->Center;
            quaternion rotation = data->Rotation;
            Color color = data->Color;

            float3 nearTopLeft = math.rotate(rotation, new float3(-widthNear, heightNear, near)) + center;
            float3 nearTopRight = math.rotate(rotation, new float3(widthNear, heightNear, near)) + center;
            float3 nearBottomLeft = math.rotate(rotation, new float3(-widthNear, -heightNear, near)) + center;
            float3 nearBottomRight = math.rotate(rotation, new float3(widthNear, -heightNear, near)) + center;

            float3 farTopLeft = math.rotate(rotation, new float3(-widthFar, heightFar, far)) + center;
            float3 farTopRight = math.rotate(rotation, new float3(widthFar, heightFar, far)) + center;
            float3 farBottomLeft = math.rotate(rotation, new float3(-widthFar, -heightFar, far)) + center;
            float3 farBottomRight = math.rotate(rotation, new float3(widthFar, -heightFar, far)) + center;

            Result[LineCount * index + 0] = new LineData(nearTopLeft, nearTopRight, color);
            Result[LineCount * index + 1] = new LineData(nearTopRight, nearBottomRight, color);
            Result[LineCount * index + 2] = new LineData(nearBottomRight, nearBottomLeft, color);
            Result[LineCount * index + 3] = new LineData(nearBottomLeft, nearTopLeft, color);

            Result[LineCount * index + 4] = new LineData(farTopLeft, farTopRight, color);
            Result[LineCount * index + 5] = new LineData(farTopRight, farBottomRight, color);
            Result[LineCount * index + 6] = new LineData(farBottomRight, farBottomLeft, color);
            Result[LineCount * index + 7] = new LineData(farBottomLeft, farTopLeft, color);

            Result[LineCount * index + 8] = new LineData(nearTopLeft, farTopLeft, color);
            Result[LineCount * index + 9] = new LineData(nearTopRight, farTopRight, color);
            Result[LineCount * index + 10] = new LineData(nearBottomRight, farBottomRight, color);
            Result[LineCount * index + 11] = new LineData(nearBottomLeft, farBottomLeft, color);
        }
    }

    public readonly struct FrustumData
    {
        public readonly float3 Center;
        public readonly quaternion Rotation;
        public readonly float Fov;
        public readonly float FarClipPlane;
        public readonly float NearClipPlane;
        public readonly float Aspect;
        public readonly Color Color;

        public FrustumData(float3 center, quaternion rotation, float fov, float farClipPlane, float nearClipPlane, float aspect, Color color)
        {
            Center = center;
            Rotation = rotation;
            Fov = fov;
            FarClipPlane = farClipPlane;
            NearClipPlane = nearClipPlane;
            Aspect = aspect;
            Color = color;
        }
    }
}