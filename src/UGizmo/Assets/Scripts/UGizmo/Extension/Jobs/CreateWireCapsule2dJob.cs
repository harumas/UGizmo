using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UGizmo.Extension.Jobs
{
    [BurstCompile]
    public unsafe struct CreateWireCapsule2dJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public Capsule2dData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public PrimitiveData* WireSemicircleResult;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public LineData* LineResult;

        private static readonly float3 up = new float3(0f, 1f, 0f);
        private static readonly float3 right = new float3(1f, 0f, 0f);
        private static readonly quaternion correctionValue;

        private const int LineCount = 2;

        static CreateWireCapsule2dJob()
        {
            correctionValue = quaternion.AxisAngle(right, math.PI);
        }

        [BurstCompile]
        public void Execute(int index)
        {
            Capsule2dData* capsuleData = GizmoDataPtr + index;

            float radius = capsuleData->Radius;
            float height = math.max(radius * 2f, capsuleData->Height);

            quaternion bottomRotation = math.mul(capsuleData->Rotation, correctionValue);

            float3 headNormal = math.rotate(capsuleData->Rotation, up);
            float3 rightNormal = math.rotate(capsuleData->Rotation, right);
            float3 offset = headNormal * (height * 0.5f - radius);

            float3 p1 = capsuleData->Center + offset + rightNormal * radius;
            float3 p2 = capsuleData->Center - offset + rightNormal * radius;
            float3 p3 = capsuleData->Center + offset - rightNormal * radius;
            float3 p4 = capsuleData->Center - offset - rightNormal * radius;

            LineResult[index * LineCount + 0] = new LineData(p1, p2, capsuleData->Color);
            LineResult[index * LineCount + 1] = new LineData(p3, p4, capsuleData->Color);

            float3 pos1 = capsuleData->Center + offset;
            float3 pos2 = capsuleData->Center - offset;

            WireSemicircleResult[index * 2 + 0] = new PrimitiveData(pos1, capsuleData->Rotation, new float3(radius), capsuleData->Color);
            WireSemicircleResult[index * 2 + 1] = new PrimitiveData(pos2, bottomRotation, new float3(radius), capsuleData->Color);
        }
    }
}