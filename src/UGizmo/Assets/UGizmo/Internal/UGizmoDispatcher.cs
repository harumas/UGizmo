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
#if UGIZMO_SRP_CORE
        private static ProfilingSampler profilingSampler;
        private static CommandBuffer commandBuffer;
#endif
        private static GizmoDrawSystem drawSystem;
        private static bool usingSRP;
#if UGIZMO_SRP_CORE
        private static bool usingHDRP;
#endif

        static UGizmoDispatcher()
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += Initialize;
#else
            Initialize();
#endif
        }

        // [InitializeOnLoad] only runs this class's static constructor in the Editor. Nothing
        // else in the codebase references UGizmoDispatcher, so in a Player build its static
        // constructor would otherwise never run (nothing touches the type to trigger it),
        // silently disabling all gizmo rendering. This method's sole purpose is to touch the
        // type early enough, which forces the CLR to run the static constructor above first.
#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureInitialized()
        {
        }
#endif

        /// <summary>
        /// Set by the UGizmo.URP assembly when it injects a ScriptableRenderPass. While this is
        /// true the endCameraRendering path below stands down, because drawing from there happens
        /// after post-processing with no depth buffer bound, which makes ZTest a no-op.
        /// </summary>
        internal static bool UseExternalRenderPass { get; set; }

        private static void Initialize()
        {
            usingSRP = GraphicsSettings.currentRenderPipeline != null;
#if UGIZMO_SRP_CORE
            usingHDRP = usingSRP && GraphicsSettings.currentRenderPipeline.GetType().FullName.Contains("HighDefinition");
#endif

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.ExitingEditMode)
                {
                    drawSystem?.ClearContinuousGizmo();
                }
            };
#endif
#if UGIZMO_SRP_CORE
            profilingSampler = new ProfilingSampler("DrawUGizmos");
            commandBuffer = CommandBufferPool.Get("UGizmo");
#endif
            drawSystem = new GizmoDrawSystem();
            drawSystem.Initialize();

            if (usingSRP)
            {
#if UGIZMO_SRP_CORE
                RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
#endif
            }
            else
            {
                Camera.onPreCull += OnPreCull_BRP;
            }
        }


        private static int previousFrame;

        /// <summary>
        /// Builds this frame's gizmo data. Safe to call once per camera - the work only runs on
        /// the first call of each frame.
        /// </summary>
        internal static void PrepareFrame(Camera camera)
        {
            // The URP injector and this class both initialize from EditorApplication.delayCall,
            // and their order is not guaranteed, so a camera can render before this runs.
            if (drawSystem == null)
            {
                return;
            }

            if (previousFrame == Time.renderedFrameCount)
            {
                return;
            }

            previousFrame = Time.renderedFrameCount;

            UGizmos.InvokeOnDraw(camera);
            drawSystem.ExecuteCreateJob();
        }

        /// <summary>
        /// Records the gizmo draw calls into a command buffer supplied by the render pipeline.
        /// </summary>
        internal static void RecordDrawCommands(CommandBuffer targetCommandBuffer)
        {
            if (drawSystem == null)
            {
                return;
            }

            drawSystem.SetCommandBuffer(targetCommandBuffer);
        }

        internal static void EndFrame()
        {
            if (drawSystem == null)
            {
                return;
            }

            drawSystem.ClearScheduler();
        }

        internal static bool ShouldRenderGizmos()
        {
#if UNITY_EDITOR
            return Handles.ShouldRenderGizmos();
#else
            return true;
#endif
        }

#if UGIZMO_SRP_CORE
        private static void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (UseExternalRenderPass)
            {
                return;
            }

            if (!ShouldRenderGizmos())
            {
                return;
            }

            if (usingHDRP)
            {
                //Run OnDrawGizmos()
                context.Submit();
            }

            PrepareFrame(camera);

            using (new ProfilingScope(commandBuffer, profilingSampler))
            {
                drawSystem.SetCommandBuffer(commandBuffer);
            }

            context.ExecuteCommandBuffer(commandBuffer);
            context.Submit();

            EndFrame();
            commandBuffer.Clear();
        }
#endif

        private static void OnPreCull_BRP(Camera camera)
        {
            if (!ShouldRenderGizmos())
            {
                return;
            }

            // The draw below targets every camera for this frame, so all the work only needs
            // to run once. Skip the remaining cameras in the same frame (e.g. split-screen).
            if (previousFrame == Time.renderedFrameCount)
            {
                return;
            }

            // Gather gizmos before culling so they are queued in time for this frame's render.
            // Works in edit mode too, where MonoBehaviour.Update does not run.
            PrepareFrame(camera);

            // null camera => drawn by ALL cameras, including the SceneView camera. A draw
            // targeted at a specific camera is never rendered by the SceneView camera, which
            // is why edit-mode gizmos require the null overload here.
            drawSystem.DrawWithCamera(null);

            EndFrame();
        }
    }
}
