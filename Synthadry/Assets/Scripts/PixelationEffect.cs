using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelationEffect : ScriptableRendererFeature
{
    [System.Serializable]
    public class PixelationSettings
    {
        public Shader pixelationShader;
        [Range(1, 128)] public float pixelSize = 8f;
    }

    public PixelationSettings settings = new PixelationSettings();
    private PixelationPass _pass;

    public override void Create()
    {
        if (settings.pixelationShader == null)
        {
            Debug.LogError("❌ PixelationEffect: Shader not assigned.");
            return;
        }

        Material material = CoreUtils.CreateEngineMaterial(settings.pixelationShader);
        _pass = new PixelationPass(material, settings)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_pass != null)
        {
            _pass.SetRenderer(renderer); // передаём renderer, cameraColorTarget берем позже
            renderer.EnqueuePass(_pass);
        }
    }

    class PixelationPass : ScriptableRenderPass
    {
        private Material _material;
        private PixelationSettings _settings;
        private RTHandle _source;
        private RTHandle _tempHandle;
        private ScriptableRenderer _renderer;  // храним renderer

        public PixelationPass(Material material, PixelationSettings settings)
        {
            _material = material;
            _settings = settings;
        }

        public void SetRenderer(ScriptableRenderer renderer)
        {
            _renderer = renderer;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            _source = _renderer.cameraColorTargetHandle;

            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref _tempHandle, descriptor, name: "_TempPixelationTex");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Pixelation");

            _material.SetFloat("_PixelSize", _settings.pixelSize);

            cmd.Blit(_source, _tempHandle.nameID, _material);
            cmd.Blit(_tempHandle.nameID, _source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            _tempHandle?.Release();
        }
    }
}
