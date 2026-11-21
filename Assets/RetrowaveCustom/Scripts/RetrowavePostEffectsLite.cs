using UnityEngine;

[ExecuteAlways]
[ImageEffectAllowedInSceneView]
[RequireComponent(typeof(Camera))]
public class RetrowavePostEffectsLite : MonoBehaviour
{
    [Header("Bloom")]
    public float threshold = 0.85f;
    public float intensity = 5f;
    [Range(0f, 1f)] public float scatter = 0.4f;

    [Header("Vignette")]
    [Range(0f, 1f)] public float vignetteIntensity = 0.3f;
    [Range(0.01f, 1f)] public float vignetteSmoothness = 0.4f;

    private Material _material;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_material == null)
        {
            var shader = Shader.Find("Hidden/Suggo Creations/Retrowave Post Effects Lite");
            if (shader == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            _material = new Material(shader);
        }

        var width = Mathf.Max(1, source.width / 4);
        var height = Mathf.Max(1, source.height / 4);
        var bright = RenderTexture.GetTemporary(width, height, 0, source.format);
        var blurHorizontal = RenderTexture.GetTemporary(width, height, 0, source.format);
        var blurVertical = RenderTexture.GetTemporary(width, height, 0, source.format);

        _material.SetFloat("_Threshold", threshold);
        Graphics.Blit(source, bright, _material, 0);

        for (var iteration = 1; iteration <= 3; iteration++)
        {
            _material.SetFloat("_BlurStep", Mathf.Lerp(0.5f, 2f, scatter) * iteration);
            Graphics.Blit(bright, blurHorizontal, _material, 1);
            Graphics.Blit(blurHorizontal, blurVertical, _material, 2);
            Graphics.Blit(blurVertical, bright);
        }

        _material.SetTexture("_BloomTex", bright);
        _material.SetFloat("_Intensity", intensity);
        _material.SetFloat("_VignetteIntensity", vignetteIntensity);
        _material.SetFloat("_VignetteSmoothness", vignetteSmoothness);
        Graphics.Blit(source, destination, _material, 3);

        RenderTexture.ReleaseTemporary(bright);
        RenderTexture.ReleaseTemporary(blurHorizontal);
        RenderTexture.ReleaseTemporary(blurVertical);
    }

    private void OnDestroy()
    {
        if (_material != null)
            DestroyImmediate(_material);
    }
}
