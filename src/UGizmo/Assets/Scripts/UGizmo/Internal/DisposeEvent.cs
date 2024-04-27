using System;
using UnityEditor;

namespace UGizmo.Internal
{
    internal class DisposeEvent : ScriptableSingleton<DisposeEvent>
    {
        public event Action Dispose;

        private void OnDisable()
        {
            Dispose?.Invoke();
            Dispose = null;
        }
    }
}