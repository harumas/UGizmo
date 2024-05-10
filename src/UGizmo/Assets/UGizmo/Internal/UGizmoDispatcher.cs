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
        private static readonly ProfilingSampler profilingSampler;
        private static readonly CommandBuffer commandBuffer;
        private static readonly GizmoRenderSystem renderSystem;
        private static bool isFirstRun = true;
        private static bool usingHDRP;
        private static bool usingSRP;

        static UGizmoDispatcher()
        {
            usingSRP = GraphicsSettings.currentRenderPipeline != null;
            usingHDRP = usingSRP && GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition");
            
#if UNITY_EDITOR
            EditorApplication.update += Initialize;
#endif
            profilingSampler = new ProfilingSampler("DrawUGizmos");
            commandBuffer = CommandBufferPool.Get();
            renderSystem = new GizmoRenderSystem();

            if (usingSRP)
            {
                RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
            }
            else
            {
                Camera.onPreCull += OnPreCull;
            }
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

            renderSystem.Initialize();
            isFirstRun = false;
        }

        private static int previousFrame;

        private static void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (isFirstRun
#if UNITY_EDITOR
                || !Handles.ShouldRenderGizmos()
#endif
               )
            {
                return;
            }

            if (usingHDRP)
            {
                context.Submit();
            }

            bool updateRenderData = previousFrame != Time.renderedFrameCount;

            if (updateRenderData)
            {
                renderSystem.ExecuteCreateJob();
            }

            using (new ProfilingScope(commandBuffer, profilingSampler))
            {
                renderSystem.SetCommandBuffer(commandBuffer);
            }

            context.ExecuteCommandBuffer(commandBuffer);
            context.Submit();

            renderSystem.ClearScheduler();
            commandBuffer.Clear();
            previousFrame = Time.renderedFrameCount;
        }

        private static void OnPreCull(Camera camera)
        {
            if (isFirstRun
#if UNITY_EDITOR
                || !Handles.ShouldRenderGizmos()
#endif
               )
            {
                return;
            }
            
            bool updateRenderData = previousFrame != Time.renderedFrameCount;

            if (updateRenderData)
            {
                renderSystem.ExecuteCreateJob();
            }

            renderSystem.DrawWithCamera(camera);

            renderSystem.ClearScheduler();
            previousFrame = Time.renderedFrameCount;
        }
    }
}