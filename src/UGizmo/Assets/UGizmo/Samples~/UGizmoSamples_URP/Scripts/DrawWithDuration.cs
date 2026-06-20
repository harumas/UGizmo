using UnityEngine;

namespace UGizmo.Samples.URP
{
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