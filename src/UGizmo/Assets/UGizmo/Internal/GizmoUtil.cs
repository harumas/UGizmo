using Unity.Burst;
using Unity.Mathematics;

namespace UGizmo.Internal
{
    // [BurstCompile]
    public static class GizmoUtil
    {
        private static readonly float3 up = new float3(0f, 1f, 0f);
        private static readonly float3 forward = new float3(0f, 0f, 1f);
        private static readonly quaternion rotate90X = quaternion.Euler(math.PI / 2f, 0f, 0f);

        // [BurstCompile]
        public static unsafe void FromUpToRotation(float3* to, out quaternion rotation)
        {
            float length = math.length(*to);
            rotation = quaternion.AxisAngle(
                angle: math.acos(math.clamp((*to).y / length, -1f, 1f)),
                axis: math.normalizesafe(new float3((*to).z, 0f, -(*to).x), forward)
            );
        }

        // [BurstCompile]
        public static unsafe void LengthAndNormalize(float3* diff, out float length, out float3 normal)
        {
            float dot = math.dot(*diff, *diff);
            length = math.sqrt(dot);
            normal = math.select(forward, *diff * (1.0f / length), dot > math.FLT_MIN_NORMAL);
        }

        // [BurstCompile]
        public static unsafe void Rotate90X(quaternion* from, out quaternion to)
        {
            to = math.mul(*from, rotate90X);
        }

        public static quaternion GetRotate90X()
        {
            return rotate90X;
        }

        // [BurstCompile]
        public static unsafe void Rotate2D(float* angle, out quaternion to)
        {
            to = quaternion.AxisAngle(forward, *angle);
        }

        // [BurstCompile]
        public static unsafe void PlaneToQuad(float* angle, out quaternion to)
        {
            to = math.mul(rotate90X, quaternion.AxisAngle(up, *angle));
        }
    }
}