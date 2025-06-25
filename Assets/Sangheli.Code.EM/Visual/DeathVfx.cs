using UnityEngine;

public class DeathVfx : MonoBehaviour
{
    public ParticleSystem ps;

    public ParticleSystemRenderer _psRenderer;

    private void OnValidate()
    {
        _psRenderer ??= ps.GetComponent<ParticleSystemRenderer>();
    }

    public void SetColor(Color color)
    {
        // ps.main.startColor.
    }

    public void SetOrder(int order) => _psRenderer.sortingOrder = order;
    public void Play() => ps.Play();
}