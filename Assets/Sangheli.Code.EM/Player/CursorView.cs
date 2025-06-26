using UnityEngine;

public class CursorView : MonoBehaviour
{
    public GameObject view;
    public GameObject viewHarvest;
    public Renderer viewRender;
    public float punchScale = 1.2f;
    public float duration = 0.2f;
    private Vector3 _originalScale;
    private static readonly int RingThickness = Shader.PropertyToID("_RingThickness");
    public MaterialPropertyBlock PropertyBlock;
    public float baseRingThickness = 0.25f;

    private void Awake()
    {
        _originalScale = view.transform.localScale;
        PropertyBlock = new MaterialPropertyBlock();
        viewRender.GetPropertyBlock(PropertyBlock);
    }

    public void SetScale(float damageScale, float harvestScale)
    {
        view.transform.localScale = _originalScale * damageScale;
        viewHarvest.transform.localScale = _originalScale * (damageScale + harvestScale);
        PropertyBlock.SetFloat(RingThickness, baseRingThickness / damageScale);
        viewRender.SetPropertyBlock(PropertyBlock);
    }

    public void Toggle(bool state)
    {
        view.gameObject.SetActive(state);
        viewHarvest.gameObject.SetActive(false);
    }

    public void DoBounceAnimation(float scale)
    {
        StopAllCoroutines();
        StartCoroutine(GameAnimation.Bounce(view.transform, _originalScale * scale, punchScale, duration));
    }
} 