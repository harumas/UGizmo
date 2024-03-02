using UnityEngine;

namespace UGizmos
{
    public readonly struct GizmoData<T> where T : unmanaged
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector3 Scale;
        public readonly Color Color;
        public readonly T CustomData;

        public GizmoData(Vector3 position, Quaternion rotation, Vector3 scale, Color color, T customData)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Color = color;
            CustomData = customData;
        }
    }
}