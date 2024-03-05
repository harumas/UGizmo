using Unity.Jobs;
using Unity.Mathematics;

namespace UGizmo.Extension.Jobs
{
    public readonly struct FrustumLineData
    {
        public readonly float3 Start;
        public readonly float3 End;
        public readonly float3 Center;
        public readonly quaternion Rotation;
        public readonly float4 Color;

        public FrustumLineData(float3 start, float3 end, float3 center, quaternion rotation, float4 color)
        {
            Start = start;
            End = end;
            Center = center;
            Rotation = rotation;
            Color = color;
        }
    }
}