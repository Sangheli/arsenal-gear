using System;
using UnityEngine;

public class DeathVfx : MonoBehaviour
{
    public ParticleSystem ps;
    public ParticleSystemRenderer psRenderer;
    private float origScale;
    private float origEmissionRate;

    private void Awake()
    {
        origScale = ps.shape.radius;
        origEmissionRate = ps.emission.rateOverTime.constant;
    }

    private void OnValidate() => psRenderer ??= ps.GetComponent<ParticleSystemRenderer>();
    public void SetOrder(int order) => psRenderer.sortingOrder = order;
    public void Play() => ps.Play();

    public void SetScale(float scale)
    {
        var shape = ps.shape;
        shape.radius = origScale * scale;
        
        var emission = ps.emission;
        emission.rateOverTime = origEmissionRate * scale;
    }
}