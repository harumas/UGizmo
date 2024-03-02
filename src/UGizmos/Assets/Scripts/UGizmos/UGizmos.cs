using UGizmos.Extension;
using UnityEngine;

namespace UGizmos
{
    public static class UGizmos
    {
        public static void DrawWireSphere(Vector3 position, float radius, Color color)
        {
            var data = new GizmoData<NoCustom>(position, Quaternion.identity, new Vector3(radius, radius, radius), color, NoCustom.None);
            Gizmo<WireSphere, NoCustom>.AddData(data);
        }
    }
}