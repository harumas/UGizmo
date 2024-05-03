using System;
using UGizmo;
using UnityEngine;

namespace Sample
{
    public class Tester:MonoBehaviour
    {


        private void Update()
        {
            UGizmos.DrawSphere(Camera.main.transform.position + Camera.main.transform.forward * 10f, 3f, Color.red);
        }
    }
}