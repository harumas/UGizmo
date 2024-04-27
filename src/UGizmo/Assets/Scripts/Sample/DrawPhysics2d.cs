using System;
using UGizmo;
using UnityEngine;

public class DrawPhysics2d : MonoBehaviour
{
    [SerializeField] private Transform ray;
    [SerializeField] private Transform circle2d;
    [SerializeField] private Transform box2d;
    [SerializeField] private Transform capsule2d;

    private void OnDrawGizmos()
    {
        UGizmos.Raycast2D(ray.position, ray.right, 5f);
        UGizmos.Circlecast2D(circle2d.position, 0.5f, circle2d.right, 5f);
        UGizmos.Boxcast2D(box2d.position, box2d.localScale, box2d.localEulerAngles.z * Mathf.Deg2Rad, Vector2.down, 5f);
        UGizmos.Capsulecast2D(capsule2d.position, capsule2d.localScale, capsule2d.localEulerAngles.z * Mathf.Deg2Rad, CapsuleDirection2D.Vertical,
            Vector2.down, 5f);
    }
}