using UGizmo.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UGizmo.URP
{
    /// <summary>
    /// Enqueues the gizmo render pass into URP's renderer every frame. Uses EnqueuePass rather
    /// than a ScriptableRendererFeature so that users do not have to edit their Renderer assets.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    internal static class UGizmoUrpInjector
    {
        private static UGizmoUrpRenderPass renderPass;

        static UGizmoUrpInjector()
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += Initialize;
#endif
        }

#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void Initialize()
        {
            if (!(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset))
            {
                return;
            }

            renderPass = new UGizmoUrpRenderPass();

            // Tell the core dispatcher to stop drawing from endCameraRendering.
            UGizmoDispatcher.UseExternalRenderPass = true;

            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        private static void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (!UGizmoDispatcher.ShouldRenderGizmos())
            {
                return;
            }

            UniversalAdditionalCameraData cameraData = camera.GetUniversalAdditionalCameraData();

            if (cameraData == null)
            {
                return;
            }

            // Build this frame's gizmo data before the renderer runs. Only does work once per
            // frame, so it is safe to call for every camera.
            UGizmoDispatcher.PrepareFrame(camera);

            cameraData.scriptableRenderer.EnqueuePass(renderPass);
        }
    }
}
