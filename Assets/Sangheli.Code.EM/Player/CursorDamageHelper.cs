using UnityEngine;

public class CursorDamageHelper
{
    private int _enemyLayer;
    private readonly Collider2D[] _enemyHits = new Collider2D[256];

    public void Init()
    {
        _enemyLayer = LayerMask.GetMask("Enemy");
    }

    public DamageResult DoDamage(Transform transform, bool isAI, ParamInfo damageParam,float damageValue, float damagePercent, float critChance, float critMultiplier, float currentDamageScaleHalf)
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            currentDamageScaleHalf,
            _enemyHits,
            _enemyLayer);
        
        int[] indices = new int[count];
        
        for (int i = 0; i < count; i++) 
            indices[i] = i;
        
        for (int i = 0; i < count; i++)
        {
            int j = Random.Range(i, count);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }
        
        float totalDamage = 0f;
        float totalShield = 0f;
        for (int i = 0; i < count; i++)
        {
            var enemy = _enemyHits[i].GetComponent<DamageReciever>();
            if (enemy == null) continue;
            var critResult = CritHelper.RollCrit(damageParam.GetValueRandomFromValue(damageValue) * damagePercent, critChance, critMultiplier);
            enemy.TakeDamage(critResult.IsCrit ? -critResult.Damage : critResult.Damage);

            if (isAI) continue;
            
            var finalDamage = G.I.HpManager.Reduce(enemy.EnemyData.DamagePerHit, doBlink: true, ignoreShield: false);
            totalShield += enemy.EnemyData.DamagePerHit - finalDamage;
            totalDamage += finalDamage;
        }
        
        return new DamageResult { PlayerDoDamage = count > 0, TotalDamage = totalDamage, TotalShieldDamage = totalShield };
    }
}