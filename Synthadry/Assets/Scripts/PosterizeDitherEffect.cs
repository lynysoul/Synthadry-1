using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PosterizeDitherEffect : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Shader shader;
        [Range(2, 32)] public float posterizeLevels = 4;
        [Range(0f, 1f)] public float ditherAmount = 0.5f;
    }

    public Settings settings = new Settings();
    private PosterizeDitherPass _pass;

    public override void Create()
    {
        if (settings.shader == null)
        {
            Debug.LogError("PosterizeDitherEffect: Shader not assigned.");
            return;
        }

        var material = CoreUtils.CreateEngineMaterial(settings.shader);
        _pass = new PosterizeDitherPass(material, settings)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_pass != null)
        {
            _pass.SetRenderer(renderer);
            renderer.EnqueuePass(_pass);
        }
    }

    class PosterizeDitherPass : ScriptableRenderPass
    {
        private Material _material;
        private Settings _settings;
        private RTHandle _source;
        private RTHandle _tempHandle;
        private ScriptableRenderer _renderer;

        public PosterizeDitherPass(Material material, Settings settings)
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
            RenderingUtils.ReAllocateIfNeeded(ref _tempHandle, descriptor, name: "_TempPosterizeDither");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("PosterizeDither");

            _material.SetFloat("_PosterizeLevels", _settings.posterizeLevels);
            _material.SetFloat("_DitherAmount", _settings.ditherAmount);

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
