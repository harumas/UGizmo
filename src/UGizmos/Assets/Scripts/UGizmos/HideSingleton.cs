using System;
using UnityEngine;

namespace UGizmos
{
    public class HideSingleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; internal set; }

        public static void SetupInstance()
        {
        }

        private void OnEnable()
        {
            Debug.Log($"OnEnable at name = {name}, id = {GetInstanceID()}");
        }

        private void OnDisable()
        {
            Debug.Log($"OnDisable at name = {name}, id = {GetInstanceID()}");
        }
    }
}