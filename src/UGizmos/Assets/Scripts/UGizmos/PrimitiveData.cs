using UnityEngine;

namespace UGizmos
{
    public readonly struct PrimitiveData
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector3 Scale;
        public readonly Color Color;

        public PrimitiveData(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Color = color;
        }
    }
}