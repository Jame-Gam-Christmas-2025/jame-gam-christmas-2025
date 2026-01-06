using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class EdgeDetection : ScriptableRendererFeature
{
    private class EdgeDetectionPass : ScriptableRenderPass
    {
        private Material material;
        private RTHandle tempTextureHandle;

        private static readonly int OutlineThicknessProperty = Shader.PropertyToID("_OutlineThickness");
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int TempTextureProperty = Shader.PropertyToID("_TempTexture");
        private static readonly int AnimationTimeProperty = Shader.PropertyToID("_AnimationTime");
        private static readonly int AnimationSpeedProperty = Shader.PropertyToID("_AnimationSpeed");

        public EdgeDetectionPass()
        {
            profilingSampler = new ProfilingSampler(nameof(EdgeDetectionPass));
        }

        public void Setup(ref EdgeDetectionSettings settings, ref Material edgeDetectionMaterial)
        {
            material = edgeDetectionMaterial;
            renderPassEvent = settings.renderPassEvent;

            float thickness = Shader.GetGlobalFloat("_ImpactFrameEdgeThickness");
            if (thickness > 0) material.SetFloat(OutlineThicknessProperty, thickness);
            else material.SetFloat(OutlineThicknessProperty, settings.outlineThickness);

            Color edgeColor = Shader.GetGlobalColor("_ImpactFrameEdgeColor");
            if (edgeColor != Color.clear) material.SetColor(OutlineColorProperty, edgeColor);
            else material.SetColor(OutlineColorProperty, settings.outlineColor);

            material.SetColor("_BackgroundColor", Shader.GetGlobalColor("_ImpactFrameBackgroundColor"));
            material.SetFloat("_ImpactIntensity", Shader.GetGlobalFloat("_ImpactFrameIntensity"));

            material.SetVector("_BlurCenter", Shader.GetGlobalVector("_BlurCenter"));
            material.SetFloat("_BlurStrength", Shader.GetGlobalFloat("_BlurStrength"));
            material.SetFloat("_BlurSamples", Shader.GetGlobalFloat("_BlurSamples"));
            material.SetFloat("_BlurRadius", Shader.GetGlobalFloat("_BlurRadius"));
            material.SetFloat("_AdditionalBlur", Shader.GetGlobalFloat("_AdditionalBlur"));

            Texture noiseTex = Shader.GetGlobalTexture("_NoiseTex");
            if (noiseTex != null) material.SetTexture("_NoiseTex", noiseTex);
            material.SetFloat("_NoiseScale", Shader.GetGlobalFloat("_NoiseScale"));
            material.SetFloat("_NoiseIntensity", Shader.GetGlobalFloat("_NoiseIntensity"));
            material.SetFloat("_EdgeStep", Shader.GetGlobalFloat("_EdgeStep"));

            float animSpeed = Shader.GetGlobalFloat("_AnimationSpeed");
            material.SetFloat(AnimationTimeProperty, Time.time);
            material.SetFloat(AnimationSpeedProperty, animSpeed);
            material.SetFloat("_AnimationSpeed", animSpeed);

            material.SetVector("_BlastWorldPosition", Shader.GetGlobalVector("_BlastWorldPosition"));
            material.SetFloat("_BlastRadius", Shader.GetGlobalFloat("_BlastRadius"));
            material.SetFloat("_BlastIntensity", Shader.GetGlobalFloat("_BlastIntensity"));
            material.SetFloat("_BlastEdgeThickness", Shader.GetGlobalFloat("_BlastEdgeThickness"));
            material.SetFloat("_BlastDebugMode", Shader.GetGlobalFloat("_BlastDebugMode"));
            material.SetFloat("_BlastShowSphere", Shader.GetGlobalFloat("_BlastShowSphere"));
        }

        private class PassData
        {
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();

            var tempDesc = renderGraph.GetTextureDesc(resourceData.activeColorTexture);
            tempDesc.name = "ImpactFrame_TempTexture";
            tempDesc.colorFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;

            var tempTexture = renderGraph.CreateTexture(tempDesc);
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Impact Frame Lines", out _))
            {
                builder.SetRenderAttachment(tempTexture, 0);
                builder.UseAllGlobalTextures(true);
                builder.AllowPassCulling(false);
                builder.SetRenderFunc((PassData _, RasterGraphContext context) => {
                    Blitter.BlitTexture(context.cmd, Vector2.one, material, 0);
                });
            }

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Impact Frame Blend", out _))
            {
                builder.UseTexture(tempTexture);
                builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                builder.UseAllGlobalTextures(true);
                builder.AllowPassCulling(false);
                builder.SetRenderFunc((PassData _, RasterGraphContext context) => {
                    material.SetTexture(TempTextureProperty, tempTexture);
                    Blitter.BlitTexture(context.cmd, Vector2.one, material, 1);
                });
            }
        }
    }

    [Serializable]
    public class EdgeDetectionSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        [Range(0, 15)] public int outlineThickness = 3;
        public Color outlineColor = Color.black;
    }

    [SerializeField] private EdgeDetectionSettings settings;
    private Material edgeDetectionMaterial;
    private EdgeDetectionPass edgeDetectionPass;

    public override void Create()
    {
        edgeDetectionPass ??= new EdgeDetectionPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Preview
            || renderingData.cameraData.cameraType == CameraType.Reflection
            || UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
            return;

        if (Shader.GetGlobalFloat("_ImpactFrameActive") <= 0.0f)
            return;

        if (edgeDetectionMaterial == null)
        {
            edgeDetectionMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/Impact Frame Edge Detection"));
            if (edgeDetectionMaterial == null)
            {
                Debug.LogWarning("Not all required materials could be created. Impact Frame Edge Detection will not render.");
                return;
            }
        }

        edgeDetectionPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color);
        edgeDetectionPass.requiresIntermediateTexture = true;
        edgeDetectionPass.Setup(ref settings, ref edgeDetectionMaterial);

        renderer.EnqueuePass(edgeDetectionPass);
    }

    override protected void Dispose(bool disposing)
    {
        edgeDetectionPass = null;
        CoreUtils.Destroy(edgeDetectionMaterial);
    }
}
