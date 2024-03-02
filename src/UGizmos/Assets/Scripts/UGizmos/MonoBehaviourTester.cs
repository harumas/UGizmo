using System;
using UnityEngine;

namespace UGizmos
{
    [ExecuteAlways]
    public class MonoBehaviourTester : MonoBehaviour
    {
        private void OnEnable()
        {
            Debug.Log(nameof(OnEnable));
        }

        private void OnDisable()
        {
            Debug.Log(nameof(OnDisable));
        }

        private void Start()
        {
            Debug.Log(nameof(Start));
        }

        private void OnDestroy()
        {
            Debug.Log(nameof(OnDestroy));
        }
    }
}