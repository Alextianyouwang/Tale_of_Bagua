using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthOccludeFeature : ScriptableRendererFeature
{
    
    class DepthTestPass : ScriptableRenderPass
    {
        private Texture2D _depthRef;
        private RenderTargetIdentifier _colSource;
        private RenderTargetHandle _temp;
        private Material _showDepthMat;
        private Material _mergeAlphaMat;
        private Material _blackMat;
        //private Material _depthMat;

        private LayerMask _objMask;
        private FilteringSettings _filteringSettings;
        private RenderQueueType _renderQueueType;


        List<ShaderTagId> _ShaderTagIdList = new List<ShaderTagId>();
        public DepthTestPass(DepthTestPassSetting setting) 
        {
            _depthRef = setting.DepthReference;
 
            _showDepthMat = setting.ShowDepthMaterial;
            _mergeAlphaMat = setting.MergeAlphaMaterial;
            _blackMat = setting.BlackMaterial;

            _renderQueueType = setting.RenderQueue;

            _objMask = setting.DepthCheckObjLayer;
            _temp.Init("_TempTex");

            _ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            _ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            _ShaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));

            //_depthMat =  CoreUtils.CreateEngineMaterial("Hidden/DepthShader");
        }

        public void SetUpSource(RenderTargetIdentifier _colSource) 
        {
            this._colSource = _colSource;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SortingCriteria sortingCriteria = (_renderQueueType == RenderQueueType.Transparent)
              ? SortingCriteria.CommonTransparent
              : renderingData.cameraData.defaultOpaqueSortFlags;
            RenderQueueRange renderQueueRange = (_renderQueueType == RenderQueueType.Transparent)
               ? RenderQueueRange.transparent
               : RenderQueueRange.opaque;
            _filteringSettings = new FilteringSettings(renderQueueRange, _objMask);
            DrawingSettings _drawingSetting = CreateDrawingSettings(_ShaderTagIdList, ref renderingData, sortingCriteria);
            _drawingSetting.overrideMaterial = _showDepthMat;
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;

            desc.depthBufferBits = 32;
            desc.colorFormat = RenderTextureFormat.Depth;
            CommandBuffer cmd = CommandBufferPool.Get();
    

            using (new ProfilingScope(cmd,new ProfilingSampler("Depth Occlude Feature"))) 
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                cmd.GetTemporaryRT(_temp.id, desc, FilterMode.Bilinear);

                //  Blit(cmd, _colSource, _temp.Identifier(), _mergeAlphaMat, 0);
                // Blit(cmd, _depthRef, _colSource);

                 context.DrawRenderers(renderingData.cullResults, ref _drawingSetting, ref _filteringSettings);


                /*Blit(cmd, _colSource, _temp.Identifier(), _mergeAlphaMat, 0);
                Blit(cmd, _temp.Identifier(), _colSource);*/
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_temp.id);
        }
    }

    DepthTestPass _depthPass;
    [SerializeField]
    public DepthTestPassSetting _depthPassSetting;
    [System.Serializable]
    public class DepthTestPassSetting 
    {
        public RenderPassEvent Event;
        public RenderQueueType RenderQueue;
        public LayerMask DepthCheckObjLayer;
        public Texture2D DepthReference;
        public Material ShowDepthMaterial;
        public Material BlackMaterial;
        public Material MergeAlphaMaterial;

    }

    public override void Create()
    {
        _depthPass = new DepthTestPass(_depthPassSetting);
        _depthPass.renderPassEvent = _depthPassSetting.Event;

    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!_depthPassSetting.DepthReference || !_depthPassSetting.ShowDepthMaterial || !_depthPassSetting.MergeAlphaMaterial)
            return;
        _depthPass.SetUpSource(renderer.cameraColorTarget);
        renderer.EnqueuePass(_depthPass);
    }
}


