using UGizmo;
using UnityEngine;

namespace UGizmo.Samples.HDRP
{
    public class DrawArrows : MonoBehaviour
    {
        [SerializeField] private Transform arrowA;
        [SerializeField] private Transform arrowB;
        [SerializeField] private Transform arrow2dA;
        [SerializeField] private Transform arrow2dB;
        [SerializeField] private Transform facingArrow2dA;
        [SerializeField] private Transform facingArrow2dB;
        [SerializeField] private Transform wireArrowA;
        [SerializeField] private Transform wireArrowB;
        [SerializeField] private Transform facingWireArrowA;
        [SerializeField] private Transform facingWireArrowB;
        [SerializeField] private Transform distanceA;
        [SerializeField] private Transform distanceB;
        [SerializeField] private Transform measureA;
        [SerializeField] private Transform measureB;

        private void OnDrawGizmos()
        {
            Color color = new Color(0.12f, 0.09f, 1f);
            UGizmos.DrawArrow(arrowA.position, arrowB.position, color);
            UGizmos.DrawArrow2d(arrow2dA.position, arrow2dB.position, Vector3.forward, color);
            UGizmos.DrawFacingArrow2d(facingArrow2dA.position, facingArrow2dB.position, color);
            UGizmos.DrawWireArrow(wireArrowA.position, wireArrowB.position, Vector3.forward, color);
            UGizmos.DrawFacingWireArrow(facingWireArrowA.position, facingWireArrowB.position, color);
            UGizmos.DrawDistance(distanceA.position, distanceB.position, color);
            UGizmos.DrawMeasure(measureA.position, measureB.position, 1, color);
        }

#if !UNITY_EDITOR
       private void Update()
        {
            Color color = new Color(0.12f, 0.09f, 1f);
            UGizmos.DrawArrow(arrowA.position, arrowB.position, color);
            UGizmos.DrawArrow2d(arrow2dA.position, arrow2dB.position, Vector3.forward, color);
            UGizmos.DrawFacingArrow2d(facingArrow2dA.position, facingArrow2dB.position, color);
            UGizmos.DrawWireArrow(wireArrowA.position, wireArrowB.position, Vector3.forward, color);
            UGizmos.DrawFacingWireArrow(facingWireArrowA.position, facingWireArrowB.position, color);
            UGizmos.DrawDistance(distanceA.position, distanceB.position, color);
            UGizmos.DrawMeasure(measureA.position, measureB.position, 1, color);
        }
#endif
    }
}