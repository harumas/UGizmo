using UGizmo.Extension;
using UGizmo.Extension.Jobs;
using UnityEngine;

namespace UGizmo
{
    public static class UGizmos
    {
        public static void DrawWireSphere(Vector3 position, float radius, Color color)
        {
            var data = new PrimitiveData(position, Quaternion.identity, new Vector3(radius, radius, radius), color);
            Gizmo<WireSphere, PrimitiveData>.AddData(data);
        }

        public static void DrawWireCube(Vector3 position, Vector3 scale, Color color)
        {
            var data = new PrimitiveData(position, Quaternion.identity, scale, color);
            Gizmo<WireCube, PrimitiveData>.AddData(data);
        }

        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            var data = new LineData(from, to, color);
            Gizmo<WireLine, LineData>.AddData(data);
        }
    }
}