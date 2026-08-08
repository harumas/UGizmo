using System.Runtime.CompilerServices;

// The URP integration lives in its own assembly so that it is compiled only when
// com.unity.render-pipelines.universal is present. It needs access to the internal
// dispatcher entry points to record gizmo draws inside a ScriptableRenderPass.
[assembly: InternalsVisibleTo("UGizmo.URP")]
