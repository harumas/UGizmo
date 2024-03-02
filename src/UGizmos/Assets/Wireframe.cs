using UnityEngine;

public class Wireframe : MonoBehaviour
{
    public static void DrawWireSphere(Vector3 pos, float radius, int subDivNum = 64)
    {
        float stepAngle = 90f / subDivNum;

        Vector3 lp1 = DrawSubWireSpherePart(pos, radius, Vector3.forward, Vector3.right, stepAngle, subDivNum);
        Vector3 lp2 = DrawSubWireSpherePart(pos, radius, Vector3.forward, -Vector3.right, stepAngle, subDivNum);

        Vector3 lp3 = DrawSubWireSpherePart(pos, radius, -Vector3.forward, Vector3.right, stepAngle, subDivNum);
        Vector3 lp4 = DrawSubWireSpherePart(pos, radius, -Vector3.forward, -Vector3.right, stepAngle, subDivNum);

        Vector3 lp5 = DrawSubWireSpherePart(pos, radius, Vector3.right, Vector3.forward, stepAngle, subDivNum);
        Vector3 lp6 = DrawSubWireSpherePart(pos, radius, Vector3.right, -Vector3.forward, stepAngle, subDivNum);

        Vector3 lp7 = DrawSubWireSpherePart(pos, radius, -Vector3.right, Vector3.forward, stepAngle, subDivNum);
        Vector3 lp8 = DrawSubWireSpherePart(pos, radius, -Vector3.right, -Vector3.forward, stepAngle, subDivNum);

        Gizmos.DrawLine(lp1, lp2);
        Gizmos.DrawLine(lp3, lp4);
        Gizmos.DrawLine(lp5, lp6);
        Gizmos.DrawLine(lp7, lp8);
    }

    private static Vector3 DrawSubWireSpherePart(Vector3 pos,
        float radius,
        Vector3 axis,
        Vector3 rotationAxis,
        float stepAngle,
        int subDivNum)
    {
        Vector3 dirVector = axis * radius;

        Vector3 lastPoint = pos;

        for (int i = subDivNum - 1; i > 1; i--)
        {
            Vector3 prevPoint = pos + Quaternion.AngleAxis((i + 1) * stepAngle, rotationAxis) * dirVector;
            lastPoint = pos + Quaternion.AngleAxis(i * stepAngle, rotationAxis) * dirVector;

            Gizmos.DrawLine(prevPoint, lastPoint);
        }

        return lastPoint;
    }
}