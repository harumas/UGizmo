using UGizmo;
using UnityEngine;

namespace UGizmo.Samples.HDRP
{
    public class DrawPhysics : MonoBehaviour
    {
        [SerializeField] private Transform ray;
        [SerializeField] private Transform sphere;
        [SerializeField] private Transform box;
        [SerializeField] private Transform capsule;

        private void OnDrawGizmos()
        {
            UGizmos.Raycast(ray.position, Vector3.forward, 10f);
            UGizmos.SphereCast(sphere.position, 0.5f, Vector3.forward, 10f);
            UGizmos.BoxCast(box.position, box.localScale * 0.5f, Vector3.forward, box.rotation, 10f);
            UGizmos.CapsuleCast(capsule.position + capsule.up * 0.5f, capsule.position - capsule.up * 0.5f, 0.5f, Vector3.forward, 10f);
        }
    }
}