using UnityEngine;

namespace UGizmo.Samples.BRP
{
    /// <summary>
    /// Gizmos drawn with a duration stay alive for that many seconds without being redrawn, so this
    /// one does not need UGizmos.OnDraw. Play mode only, because it is driven by InvokeRepeating.
    /// </summary>
    public class DrawWithDuration : MonoBehaviour
    {
        [SerializeField] private float duration;

        private void Start()
        {
            //Display at 1-second intervals
            InvokeRepeating(nameof(DrawGizmoWithDuration), 0f, duration + 1f);
        }

        private void DrawGizmoWithDuration()
        {
            UGizmos.DrawSphere(transform.position, 2f, Color.yellow, duration);
            UGizmos.DrawCube(transform.position + new Vector3(3f, 0f, 0f), Quaternion.identity, Vector3.one, Color.yellow, duration);
        }
    }
}
