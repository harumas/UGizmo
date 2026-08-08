using UnityEngine;

namespace UGizmo.Samples.BRP
{
    /// <summary>
    /// These samples subscribe to UGizmos.OnDraw rather than using MonoBehaviour.OnDrawGizmos.
    /// OnDrawGizmos does work in the Built-in Render Pipeline, but it runs after Camera.onPreCull,
    /// where UGizmo collects and submits its data, so anything drawn from it shows up one frame
    /// late. It is also editor-only, which is why the URP and HDRP samples repeat every draw call
    /// inside "#if !UNITY_EDITOR Update()" to keep working in a build.
    /// UGizmos.OnDraw fires once per frame from inside the render loop, so there is no frame of
    /// lag and no duplicated code path, and it works in play mode, edit mode and builds alike.
    /// </summary>
    [ExecuteAlways]
    public class DrawPrimitives : MonoBehaviour
    {
        [SerializeField] private Color color;
        [SerializeField] private Transform cube;
        [SerializeField] private Transform wireCube;
        [SerializeField] private Transform sphere;
        [SerializeField] private Transform wireSphere;
        [SerializeField] private Transform capsule;
        [SerializeField] private Transform wireCapsule;
        [SerializeField] private Transform cylinder;
        [SerializeField] private Transform wireCylinder;
        [SerializeField] private Transform cone;
        [SerializeField] private Transform wireCone;
        [SerializeField] private Transform plane;
        [SerializeField] private Transform wirePlane;

        private void OnEnable()
        {
            UGizmos.OnDraw += Draw;
        }

        private void OnDisable()
        {
            UGizmos.OnDraw -= Draw;
        }

        private void Draw(Camera camera)
        {
            UGizmos.DrawCube(cube.position, cube.rotation, cube.localScale, color);
            UGizmos.DrawWireCube(wireCube.position, wireCube.rotation, wireCube.localScale, color);

            UGizmos.DrawSphere(sphere.position, 0.5f, color);
            UGizmos.DrawWireSphere(wireSphere.position, 0.5f, color);

            UGizmos.DrawCapsule(capsule.position, capsule.up, 2f, 0.5f, color);
            UGizmos.DrawWireCapsule(wireCapsule.position, wireCapsule.up, 2f, 0.5f, color);

            UGizmos.DrawCylinder(cylinder.position, cylinder.rotation, cylinder.localScale, color);
            UGizmos.DrawWireCylinder(wireCylinder.position, wireCylinder.rotation, new Vector3(0.5f, 1f, 0.5f), color);

            UGizmos.DrawCone(cone.position, cone.rotation, cone.localScale, color);
            UGizmos.DrawWireCone(wireCone.position, wireCone.rotation, wireCone.localScale, color);

            Vector2 scale = new Vector2(plane.localScale.x, plane.localScale.z);
            UGizmos.DrawPlane(plane.position, plane.rotation, scale, color);

            scale = new Vector2(wirePlane.localScale.x, wirePlane.localScale.z);
            UGizmos.DrawWirePlane(wirePlane.position, wirePlane.rotation, scale, color);
        }
    }
}
