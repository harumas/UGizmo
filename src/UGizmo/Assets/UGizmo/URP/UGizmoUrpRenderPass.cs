using UGizmo.Internal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif

namespace UGizmo.URP
{
    /// <summary>
    /// Draws the gizmos inside URP's own render pass list, so that the camera depth attachment is
    /// bound and ZTest actually works. Drawing from RenderPipelineManager.endCameraRendering
    /// instead leaves no depth attachment bound, which silently turns every ZTest state into a
    /// no-op and makes both passes of the two-pass gizmo shaders render on top of each other.
    /// </summary>
    internal sealed class UGizmoUrpRenderPass : ScriptableRenderPass
    {
        public UGizmoUrpRenderPass()
        {
            // After post-processing so effects like Bloom do not alter the gizmos, but before the
            // final blit, so we are still on an intermediate target that has depth attached.
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        // URP 17 (Unity 6) dropped the whole compatibility-mode API - Execute, OnCameraSetup and
        // ConfigureTarget no longer exist on ScriptableRenderPass, leaving only RecordRenderGraph.
        // Older URP versions are the exact opposite, so the two paths are mutually exclusive.
#if UNITY_6000_0_OR_NEWER
        private class PassData
        {
            public TextureHandle ColorTarget;
            public TextureHandle DepthTarget;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

            // An unsafe pass lets the existing CommandBuffer-based drawing code run unchanged.
            using (IUnsafeRenderGraphBuilder builder = renderGraph.AddUnsafePass("UGizmo", out PassData passData))
            {
                passData.ColorTarget = resourceData.activeColorTexture;
                passData.DepthTarget = resourceData.activeDepthTexture;

                // ReadWrite, not Write: the gizmos alpha-blend against what is already in the
                // buffer, so the existing contents must be loaded rather than discarded.
                builder.UseTexture(passData.ColorTarget, AccessFlags.ReadWrite);

                // ZWrite is off in every gizmo shader, so depth is only ever tested.
                builder.UseTexture(passData.DepthTarget, AccessFlags.Read);

                // Gizmo draws are never referenced by later passes, so the graph would cull this.
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, UnsafeGraphContext context) =>
                {
                    CommandBuffer commandBuffer = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                    commandBuffer.SetRenderTarget(data.ColorTarget, data.DepthTarget);

                    UGizmoDispatcher.RecordDrawCommands(commandBuffer);
                    UGizmoDispatcher.EndFrame();
                });
            }
        }
#else
        // Deliberately no ConfigureTarget/OnCameraSetup override here. OnCameraSetup runs for every
        // queued pass up front, before any of them execute, so a target captured there is stale by
        // the time this pass runs: PostProcessPass calls SwapColorBuffer, ping-ponging the camera
        // colour between _CameraColorAttachmentA and B. Pinning the target would draw the gizmos
        // into the buffer FinalBlit does not read, making them silently invisible. Leaving
        // overrideCameraTarget unset makes URP bind whichever colour and depth targets are current
        // when the pass actually executes, which SwapColorBuffer keeps up to date.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get("UGizmo");

            UGizmoDispatcher.RecordDrawCommands(commandBuffer);

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);

            UGizmoDispatcher.EndFrame();
        }
#endif
    }
}
