using UnityEngine;

public static class CritHelper
{
    public static CritResult RollCrit(float baseDamage, float critChance, float critMultiplier)
    {
        bool isCrit = Random.value < critChance / 100f;
        float finalDamage = isCrit ? baseDamage * critMultiplier : baseDamage;
        return new CritResult { Damage = finalDamage, IsCrit = isCrit };
    }
} 