using UnityEngine;

namespace UGizmo.Samples.URP
{
    /// <summary>
    /// Sample global application input controller.
    /// Toggles UGizmo on/off when the Q key is pressed.
    /// </summary>
    public class UGizmoSampleInput : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if (GameObject.Find("UGizmoSampleInput") != null) return;
            var go = new GameObject("UGizmoSampleInput");
            go.AddComponent<UGizmoSampleInput>();
            DontDestroyOnLoad(go);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UGizmos.enabled = !UGizmos.enabled;
            }
        }
    }
}
