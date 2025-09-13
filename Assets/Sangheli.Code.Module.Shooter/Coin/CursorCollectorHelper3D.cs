using UnityEngine;

public class CursorCollectorHelper3D
{
    private readonly Collider[] _energyHits = new Collider[256]; // 3D коллайдеры
    private int _energyLayer;

    public void Init()
    {
        _energyLayer = LayerMask.GetMask("Energy");
    }

    public void CollectEnergy(Transform transform, float collectRadius, float currentDamageScaleHalf)
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            collectRadius,
            _energyHits,
            _energyLayer);

        for (int i = 0; i < count; i++)
        {
            var energy = _energyHits[i].GetComponent<EnergyView>();
            
            if(energy == null)
                energy = _energyHits[i].transform.parent.GetComponent<EnergyView>();
            
            if (energy == null)
                continue;
            energy.Collect(transform, currentDamageScaleHalf, collectRadius);
        }
    }
}