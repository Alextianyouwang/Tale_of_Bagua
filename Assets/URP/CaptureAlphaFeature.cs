using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CaptureAlphaFeature : ScriptableRendererFeature
{
    private CaptureAlphaPass captureAlphaPass;

    public override void Create()
    {
        captureAlphaPass = new CaptureAlphaPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //if ( renderingData.cameraData.isSceneViewCamera)
        {
            captureAlphaPass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(captureAlphaPass);
        }
    }
}
public class CaptureAlphaPass : ScriptableRenderPass
{
    private static readonly string k_RenderTag = "Render Alpha To RGB";
    private Material alphaToRGBMaterial;
    private RenderTargetIdentifier currentTarget;

    public void Setup(RenderTargetIdentifier target)
    {
        currentTarget = target;
    }

    public CaptureAlphaPass()
    {
        renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        Shader shader = Shader.Find("Utility/AlphaToRGB");
        if (shader == null)
        {
            Debug.LogError("Cannot find the AlphaToRGB shader. Please ensure it's in the project.");
            return;
        }

        //alphaToRGBMaterial = CoreUtils.CreateEngineMaterial(shader);
        alphaToRGBMaterial = new Material(shader);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(k_RenderTag);

        using (new ProfilingScope(cmd, new ProfilingSampler(k_RenderTag)))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            var renderer = renderingData.cameraData.renderer;

            cmd.SetRenderTarget(currentTarget);
            cmd.ClearRenderTarget(true, true, Color.clear);

            // Render objects with the custom shader into the currentTarget.
            var drawSettings = CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, SortingCriteria.CommonOpaque);
            drawSettings.overrideMaterial = alphaToRGBMaterial;
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
