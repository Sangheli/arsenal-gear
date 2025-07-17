using UnityEngine;

public class CursorCollectorHelper
{
    private readonly Collider2D[] _energyHits = new Collider2D[256];
    private int _energyLayer;

    public void Init()
    {
        _energyLayer = LayerMask.GetMask("Energy");
    }

    public void CollectEnergy(Transform transform, float collectRadius, float currentDamageScaleHalf)
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            collectRadius,
            _energyHits,
            _energyLayer);
        
        for (int i = 0; i < count; i++)
        {
            var energy = _energyHits[i].GetComponent<EnergyView>();
            if (energy == null)
                continue;
            energy.Collect(transform, currentDamageScaleHalf);
        }
    }
}