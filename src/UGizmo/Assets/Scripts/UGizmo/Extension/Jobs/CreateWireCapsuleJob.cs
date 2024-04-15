using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UGizmo.Extension.Jobs
{
    [BurstCompile]
    public unsafe struct CreateWireCapsuleJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public CapsuleData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public PrimitiveData* HemisphereResult;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public LineData* LineResult;

        private static readonly quaternion vertical;
        private static readonly float3 front = new float3(0f, 0f, 1f);
        private static readonly float3 right = new float3(1f, 0f, 0f);
        private const int LineCount = 4;

        static CreateWireCapsuleJob()
        {
            vertical = quaternion.AxisAngle(new float3(1f, 0f, 0f), math.PI);
        }

        [BurstCompile]
        public void Execute(int index)
        {
            CapsuleData* capsuleData = GizmoDataPtr + index;

            float radius = capsuleData->Radius;
            float height = math.max(radius * 2f, capsuleData->Height);

            float cos = (1 + math.clamp(capsuleData->UpAxis.y, -1f, 1f)) * 0.5f;
            float3 axis = math.normalizesafe(new float3(capsuleData->UpAxis.z, 0f, -capsuleData->UpAxis.x), new float3(0f, 0f, 1f));

            float4 value = new float4(axis * math.sqrt(1f - cos), math.sqrt(cos));
            quaternion topRotation = new quaternion(value);
            quaternion bottomRotation = math.mul(topRotation, vertical);

            float3 sphereOffset = capsuleData->UpAxis * (height * 0.5f - radius);
            float3 pos1 = capsuleData->Center + sphereOffset;
            float3 pos2 = capsuleData->Center - sphereOffset;

            HemisphereResult[index * 2 + 0] = new PrimitiveData(pos1, topRotation, new float3(radius), capsuleData->Color);
            HemisphereResult[index * 2 + 1] = new PrimitiveData(pos2, bottomRotation, new float3(radius), capsuleData->Color);

            float3 rotateFront = math.rotate(topRotation, front * radius);
            float3 rotateRight = math.rotate(topRotation, right * radius);

            float3 p1 = rotateFront + pos1;
            float3 p2 = rotateFront + pos2;
            LineResult[LineCount * index + 0] = new LineData(p1, p2, capsuleData->Color);

            float3 p3 = -rotateFront + pos1;
            float3 p4 = -rotateFront + pos2;
            LineResult[LineCount * index + 1] = new LineData(p3, p4, capsuleData->Color);

            float3 p5 = rotateRight + pos1;
            float3 p6 = rotateRight + pos2;
            LineResult[LineCount * index + 2] = new LineData(p5, p6, capsuleData->Color);

            float3 p7 = -rotateRight + pos1;
            float3 p8 = -rotateRight + pos2;
            LineResult[LineCount * index + 3] = new LineData(p7, p8, capsuleData->Color);
        }
    }
}