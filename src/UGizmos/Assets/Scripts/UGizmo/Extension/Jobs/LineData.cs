using Unity.Mathematics;
using UnityEngine;

namespace UGizmo
{
    public struct LineData
    {
        public readonly Vector3 Start;
        public readonly Vector3 End;
        public readonly Color Color;

        public LineData(float3 start, float3 end, Color color)
        {
            Start = start;
            End = end;
            Color = color;
        }
    }
}