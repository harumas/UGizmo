using UGizmo;
using UnityEngine;

namespace UGizmo.Samples.HDRP
{
    public class DrawPrimitives2D : MonoBehaviour
    {
        [SerializeField] private Color color;
        [SerializeField] private Transform box2D;
        [SerializeField] private Transform wireBox2D;
        [SerializeField] private Transform circle2D;
        [SerializeField] private Transform wireCircle2D;
        [SerializeField] private Transform triangle2D;
        [SerializeField] private Transform wireTriangle2D;
        [SerializeField] private Transform capsule2D;
        [SerializeField] private Transform wireCapsule2D;


        private void OnDrawGizmos()
        {
            UGizmos.DrawBox2D(box2D.position, box2D.rotation, box2D.localScale, color);
            UGizmos.DrawWireBox2D(wireBox2D.position, wireBox2D.rotation, wireBox2D.localScale, color);

            UGizmos.DrawCircle2D(circle2D.position, circle2D.rotation, 0.5f, color);
            UGizmos.DrawWireCircle2D(wireCircle2D.position, wireCircle2D.rotation, 0.5f, color);

            UGizmos.DrawTriangle2D(triangle2D.position, triangle2D.localEulerAngles.z * Mathf.Deg2Rad, triangle2D.localScale, color);
            UGizmos.DrawWireTriangle2D(wireTriangle2D.position, wireTriangle2D.localEulerAngles.z * Mathf.Deg2Rad, wireTriangle2D.localScale, color);

            UGizmos.DrawCapsule2D(capsule2D.position, capsule2D.rotation, 2f, 0.5f, color);
            UGizmos.DrawWireCapsule2D(wireCapsule2D.position, wireCapsule2D.rotation, 2f, 0.5f, color);
        }
    }
}