using Unity.Mathematics;
using UnityEngine;

namespace UGizmo
{
    public readonly struct RenderData
    {
        public readonly float4x4 Matrix;
        public readonly Color Color;

        public RenderData(in float4x4 matrix, in Color color)
        {
            this.Matrix = matrix;
            this.Color = color;
        }
    }
}