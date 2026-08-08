using UnityEngine;

namespace UGizmo.Samples.BRP
{
    /// <summary>
    /// Drives <see cref="UGizmos.depthTest"/> from a checkbox in the inspector, and draws a
    /// wireframe sphere that intersects whatever this sits on, so the effect is easy to see: with
    /// depth testing on, the half behind the geometry fades out.
    ///
    /// [ExecuteAlways] matters here. UGizmos.depthTest is a static field, so it resets to false
    /// every time scripts recompile, and a plain MonoBehaviour gets no Update in edit mode - the
    /// toggle would appear to do nothing until you entered play mode.
    /// </summary>
    [ExecuteAlways]
    public class DepthTestToggle : MonoBehaviour
    {
        [SerializeField] private bool depthTest = true;
        [SerializeField] private float radius = 1.5f;
        [SerializeField] private Color color = new Color(0.12f, 0.75f, 1f);

        private void OnEnable()
        {
            UGizmos.OnDraw += Draw;
            Apply();
        }

        private void OnDisable()
        {
            UGizmos.OnDraw -= Draw;
        }

        /// <summary>
        /// Applies the value as soon as the checkbox is changed in the inspector.
        /// </summary>
        private void OnValidate()
        {
            Apply();
        }

        /// <summary>
        /// Re-applies every frame so the setting survives a domain reload, which would otherwise
        /// silently reset it back to false.
        /// </summary>
        private void Update()
        {
            Apply();
        }

        private void Apply()
        {
            UGizmos.depthTest = depthTest;
        }

        private void Draw(Camera camera)
        {
            UGizmos.DrawWireSphere(transform.position, radius, color);
        }
    }
}
