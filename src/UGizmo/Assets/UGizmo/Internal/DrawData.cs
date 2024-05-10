using Unity.Mathematics;
using UnityEngine;

namespace UGizmo.Internal
{
    internal readonly struct DrawData
    {
        public readonly float4x4 Matrix;
        public readonly Color Color;

        public DrawData(in float4x4 matrix, in Color color)
        {
            this.Matrix = matrix;
            this.Color = color;
        }
    }
}