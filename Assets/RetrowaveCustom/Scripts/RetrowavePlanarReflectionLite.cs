using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class RetrowavePlanarReflectionLite : MonoBehaviour
{
    private const int ReflectionLayer = 31;

    [Min(256)] public int textureSize = 1024;
    [Range(0f, 1f)] public float reflectionStrength = 0.65f;

    private MeshRenderer _renderer;
    private Camera _reflectionCamera;
    private RenderTexture _reflectionTexture;
    private MaterialPropertyBlock _propertyBlock;

    private void OnEnable()
    {
        _renderer = GetComponent<MeshRenderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void OnWillRenderObject()
    {
        var currentCamera = Camera.current;
        if (currentCamera == null || currentCamera == _reflectionCamera || _renderer == null)
            return;

        var normal = transform.up;
        if (Vector3.Dot(normal, currentCamera.transform.position - transform.position) <= 0)
            return;

        EnsureCamera();
        EnsureTexture();

        _reflectionCamera.CopyFrom(currentCamera);
        _reflectionCamera.enabled = false;
        _reflectionCamera.targetTexture = _reflectionTexture;
        _reflectionCamera.depthTextureMode = DepthTextureMode.None;
        _reflectionCamera.cullingMask = currentCamera.cullingMask & ~(1 << ReflectionLayer);
        _reflectionCamera.useOcclusionCulling = false;
        _reflectionCamera.allowMSAA = false;

        var reflection = Matrix4x4.identity;
        reflection.m00 = 1f - 2f * normal.x * normal.x;
        reflection.m01 = -2f * normal.x * normal.y;
        reflection.m02 = -2f * normal.x * normal.z;
        reflection.m10 = -2f * normal.y * normal.x;
        reflection.m11 = 1f - 2f * normal.y * normal.y;
        reflection.m12 = -2f * normal.y * normal.z;
        reflection.m20 = -2f * normal.z * normal.x;
        reflection.m21 = -2f * normal.z * normal.y;
        reflection.m22 = 1f - 2f * normal.z * normal.z;

        var planeOffset = -2f * Vector3.Dot(normal, transform.position);
        reflection.m03 = planeOffset * normal.x;
        reflection.m13 = planeOffset * normal.y;
        reflection.m23 = planeOffset * normal.z;

        _reflectionCamera.worldToCameraMatrix = currentCamera.worldToCameraMatrix * reflection;
        _reflectionCamera.projectionMatrix = currentCamera.projectionMatrix;

        var viewPosition = _reflectionCamera.worldToCameraMatrix.MultiplyPoint(transform.position);
        var viewNormal = _reflectionCamera.worldToCameraMatrix.MultiplyVector(normal).normalized;
        var clipPlane = new Vector4(viewNormal.x, viewNormal.y, viewNormal.z, -Vector3.Dot(viewPosition, viewNormal));
        _reflectionCamera.projectionMatrix = _reflectionCamera.CalculateObliqueMatrix(clipPlane);

        var previousLayer = gameObject.layer;
        var previousInvertCulling = GL.invertCulling;
        gameObject.layer = ReflectionLayer;
        GL.invertCulling = true;
        try
        {
            _reflectionCamera.Render();
        }
        finally
        {
            GL.invertCulling = previousInvertCulling;
            gameObject.layer = previousLayer;
        }

        _renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetTexture("_ReflectionTex", _reflectionTexture);
        _propertyBlock.SetFloat("_ReflectionStrength", reflectionStrength);
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    private void EnsureCamera()
    {
        if (_reflectionCamera != null)
            return;

        var cameraObject = new GameObject("Retrowave Planar Reflection Camera");
        cameraObject.hideFlags = HideFlags.HideAndDontSave;
        _reflectionCamera = cameraObject.AddComponent<Camera>();
        _reflectionCamera.enabled = false;
    }

    private void EnsureTexture()
    {
        if (_reflectionTexture != null && _reflectionTexture.width == textureSize && _reflectionTexture.height == textureSize)
            return;

        if (_reflectionTexture != null)
        {
            _reflectionTexture.Release();
            DestroyImmediate(_reflectionTexture);
        }

        _reflectionTexture = new RenderTexture(textureSize, textureSize, 24, RenderTextureFormat.DefaultHDR)
        {
            name = "Retrowave Planar Reflection",
            filterMode = FilterMode.Bilinear,
            useMipMap = false,
            autoGenerateMips = false
        };
        _reflectionTexture.Create();
    }

    private void OnDisable()
    {
        if (_reflectionCamera != null)
            DestroyImmediate(_reflectionCamera.gameObject);

        if (_reflectionTexture != null)
        {
            _reflectionTexture.Release();
            DestroyImmediate(_reflectionTexture);
        }
    }
}
