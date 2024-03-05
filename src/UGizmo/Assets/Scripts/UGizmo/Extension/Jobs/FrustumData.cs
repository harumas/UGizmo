using UnityEngine;

namespace UGizmo.Extension.Jobs
{
    public readonly struct FrustumData
    {
        public readonly Vector3 Center;
        public readonly Quaternion Rotation;
        public readonly float Fov;
        public readonly float FarClipPlane;
        public readonly float NearClipPlane;
        public readonly float Aspect;
        public readonly Color Color;

        public FrustumData(Vector3 center, Quaternion rotation, float fov, float farClipPlane, float nearClipPlane, float aspect, Color color)
        {
            Center = center;
            Rotation = rotation;
            Fov = fov;
            FarClipPlane = farClipPlane;
            NearClipPlane = nearClipPlane;
            Aspect = aspect;
            Color = color;
        }
    }
}