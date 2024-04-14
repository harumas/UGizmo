using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UGizmo.Extension.Jobs
{
    [BurstCompile]
    internal unsafe struct CreateWireTriangleJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        [ReadOnly]
        public PrimitiveData* GizmoDataPtr;

        [NativeDisableUnsafePtrRestriction]
        [WriteOnly]
        public LineData* Result;

        private const int LineCount = 3;

        private static readonly float3[] vertices;

        static CreateWireTriangleJob()
        {
            vertices = new[]
            {
                new float3(0f, 1f / 1.73205080757f, 0f),
                new float3(-0.5f, -1f / (2f * 1.73205080757f), 0f),
                new float3(0.5f, -1f / (2f * 1.73205080757f), 0f),
            };
        }

        [BurstCompile]
        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            PrimitiveData* planeData = GizmoDataPtr + index;

            float3 position = planeData->Position;
            float3 p1 = math.rotate(planeData->Rotation, vertices[0] * planeData->Scale) + position;
            float3 p2 = math.rotate(planeData->Rotation, vertices[1] * planeData->Scale) + position;
            float3 p3 = math.rotate(planeData->Rotation, vertices[2] * planeData->Scale) + position;

            Result[LineCount * index + 0] = new LineData(p1, p2, planeData->Color);
            Result[LineCount * index + 1] = new LineData(p2, p3, planeData->Color);
            Result[LineCount * index + 2] = new LineData(p3, p1, planeData->Color);
        }
    }
}