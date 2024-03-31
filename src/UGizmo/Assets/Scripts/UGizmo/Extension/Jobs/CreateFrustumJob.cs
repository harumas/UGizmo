using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace UGizmo.Extension.Jobs
{
    [BurstCompile]
    internal unsafe struct CreateFrustumJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public FrustumData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public FrustumLineData* Result;

        private const int LineCount = 12;

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            FrustumData* data = GizmoDataPtr + index;

            float near = data->NearClipPlane;
            float far = data->FarClipPlane;
            float fov = data->Fov;
            float aspect = data->Aspect;
            float height = math.tan(math.radians(fov * 0.5f));

            float heightNear = height * near;
            float widthNear = heightNear * aspect;

            float heightFar = height * far;
            float widthFar = heightFar * aspect;

            float3 nearTopLeft = new float3(-widthNear, heightNear, near);
            float3 nearTopRight = new float3(widthNear, heightNear, near);
            float3 nearBottomLeft = new float3(-widthNear, -heightNear, near);
            float3 nearBottomRight = new float3(widthNear, -heightNear, near);

            float3 farTopLeft = new float3(-widthFar, heightFar, far);
            float3 farTopRight = new float3(widthFar, heightFar, far);
            float3 farBottomLeft = new float3(-widthFar, -heightFar, far);
            float3 farBottomRight = new float3(widthFar, -heightFar, far);

            float3 center = data->Center;
            quaternion rotation = data->Rotation;
            Color originColor = data->Color;

            Result[LineCount * index + 0] = new FrustumLineData(nearTopLeft, nearTopRight, center, rotation, originColor);
            Result[LineCount * index + 1] = new FrustumLineData(nearTopRight, nearBottomRight, center, rotation, originColor);
            Result[LineCount * index + 2] = new FrustumLineData(nearBottomRight, nearBottomLeft, center, rotation, originColor);
            Result[LineCount * index + 3] = new FrustumLineData(nearBottomLeft, nearTopLeft, center, rotation, originColor);

            Result[LineCount * index + 4] = new FrustumLineData(farTopLeft, farTopRight, center, rotation, originColor);
            Result[LineCount * index + 5] = new FrustumLineData(farTopRight, farBottomRight, center, rotation, originColor);
            Result[LineCount * index + 6] = new FrustumLineData(farBottomRight, farBottomLeft, center, rotation, originColor);
            Result[LineCount * index + 7] = new FrustumLineData(farBottomLeft, farTopLeft, center, rotation, originColor);

            Result[LineCount * index + 8] = new FrustumLineData(nearTopLeft, farTopLeft, center, rotation, originColor);
            Result[LineCount * index + 9] = new FrustumLineData(nearTopRight, farTopRight, center, rotation, originColor);
            Result[LineCount * index + 10] = new FrustumLineData(nearBottomRight, farBottomRight, center, rotation, originColor);
            Result[LineCount * index + 11] = new FrustumLineData(nearBottomLeft, farBottomLeft, center, rotation, originColor);
        }
    }
}