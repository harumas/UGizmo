using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace UGizmo.Internal
{
    [StructLayout(LayoutKind.Sequential)]
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