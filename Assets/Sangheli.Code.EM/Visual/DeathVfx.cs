using UnityEngine;

public class DeathVfx : MonoBehaviour
{
    public ParticleSystem ps;
    public ParticleSystemRenderer psRenderer;

    private void OnValidate() => psRenderer ??= ps.GetComponent<ParticleSystemRenderer>();
    public void SetOrder(int order) => psRenderer.sortingOrder = order;
    public void Play() => ps.Play();
}