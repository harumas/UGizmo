using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace UGizmo.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DrawData
    {
        public readonly float m00, m10, m20;
        public readonly float m01, m11, m21;
        public readonly float m02, m12, m22;
        public readonly float m03, m13, m23;
        
        public readonly Color32 ColorRaw;

        public DrawData(in float4x4 matrix, in Color color)
        {
            m00 = matrix.c0.x; m10 = matrix.c0.y; m20 = matrix.c0.z;
            m01 = matrix.c1.x; m11 = matrix.c1.y; m21 = matrix.c1.z;
            m02 = matrix.c2.x; m12 = matrix.c2.y; m22 = matrix.c2.z;
            m03 = matrix.c3.x; m13 = matrix.c3.y; m23 = matrix.c3.z;
            ColorRaw = color;
        }
    }
}