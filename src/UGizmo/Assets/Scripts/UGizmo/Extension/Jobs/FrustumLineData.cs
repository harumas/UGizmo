using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace UGizmo.Extension.Jobs
{
    public readonly struct FrustumLineData
    {
        public readonly float3 Start;
        public readonly float3 End;
        public readonly float3 Center;
        public readonly quaternion Rotation;
        public readonly Color Color;

        public FrustumLineData(float3 start, float3 end, float3 center, quaternion rotation, Color color)
        {
            Start = start;
            End = end;
            Center = center;
            Rotation = rotation;
            Color = color;
        }
    }
}