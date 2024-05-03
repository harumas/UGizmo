using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UGizmo.Internal
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    internal static class UGizmoDispatcher
    {
        private static bool isFirstRun = true;
        private static CommandBuffer commandBuffer;

        static UGizmoDispatcher()
        {
#if UNITY_EDITOR
            EditorApplication.update += Initialize;
#endif

            commandBuffer = new CommandBuffer();
            RenderPipelineManager.endContextRendering += OnEndCameraRendering;
        }

#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void Initialize()
        {
            if (!isFirstRun)
            {
                return;
            }

            GizmoRendererSystem.Initialize();
            isFirstRun = false;
        }


        private static void OnEndCameraRendering(ScriptableRenderContext context, List<Camera> _)
        {
            GizmoRendererSystem.SetupCommandBuffer(commandBuffer);

            context.ExecuteCommandBuffer(commandBuffer);
            context.Submit();

            commandBuffer.Clear();
        }
    }
}