using System;
using UnityEngine;

public class DeathVfx : MonoBehaviour
{
    public ParticleSystem ps;
    public ParticleSystemRenderer psRenderer;
    private float _origScale;
    private float _origEmissionRate;
    public Color colorMax;

    private void Awake()
    {
        var main = ps.main;
        colorMax = main.startColor.colorMax;
        _origScale = ps.shape.radius;
        _origEmissionRate = ps.emission.rateOverTime.constant;
    }

    private void OnValidate() => psRenderer ??= ps.GetComponent<ParticleSystemRenderer>();
    public void SetOrder(int order) => psRenderer.sortingOrder = order;
    public void Play() => ps.Play();

    public void SetScale(float scale)
    {
        var shape = ps.shape;
        shape.radius = _origScale * scale;
        
        var emission = ps.emission;
        emission.rateOverTime = _origEmissionRate * scale;
    }

    public void SetColor(Color color)
    {
        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color,colorMax);
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }
}