using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeathVfx : MonoBehaviour
{
    public ParticleSystem ps;
    public ParticleSystemRenderer psRenderer;
    private float _origScale;
    private float _origEmissionRate;
    public Color colorMax;
    
    public List<ParticleSystemRenderer> psRendererArr;
    private int[] _origOrders;

    private void Awake()
    {
        var renderers = GetComponentsInChildren<ParticleSystemRenderer>().ToList();
        if (psRenderer != null && !renderers.Contains(psRenderer))
            renderers.Add(psRenderer);
        psRendererArr = renderers;
        _origOrders = psRendererArr.Select(r => r.sortingOrder).ToArray();
        
        var main = ps.main;
        colorMax = main.startColor.colorMax;
        _origScale = ps.shape.radius;
        _origEmissionRate = ps.emission.rateOverTime.constant;
    }

    private void OnValidate() => psRenderer ??= ps.GetComponent<ParticleSystemRenderer>();

    public void SetOrder(int baseOrder)
    {
        if(_origOrders.Length == 0)
            return;
        
        int min =  _origOrders.Min();
        for (int i = 0; i < psRendererArr.Count; i++)
        {
            int offset = _origOrders[i] - min;
            psRendererArr[i].sortingOrder = baseOrder + offset;
        }
    }

    public void Play() => ps.Play();

    public void SetScale(float scale)
    {
        var shape = ps.shape;
        shape.radius = _origScale * scale;
        
        var emission = ps.emission;
        emission.rateOverTime = _origEmissionRate * scale;
    }

    public void SetColor(Color color, Color colorMax)
    {
        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color,colorMax);
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
        for (var i = 0; i < transform.childCount; i++) transform.GetChild(i).localScale = scale;
    }
}