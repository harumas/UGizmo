using System;
using UnityEngine;

namespace UGizmo.Samples.URP
{
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

        private void OnDrawGizmos()
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