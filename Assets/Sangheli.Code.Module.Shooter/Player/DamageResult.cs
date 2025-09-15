// Структура для возврата результата атаки

using UnityEngine;

public struct DamageResult
{
    public bool HasRecivedDamage => TotalDamage>0.0001f;
    public bool HasRecivedShieldDamage => TotalShieldDamage>0.0001f;
    public bool HasAnyDamage => HasRecivedDamage || HasRecivedShieldDamage;
    public bool PlayerDoDamage;
    public float TotalDamage;
    public float TotalShieldDamage;
    public Vector3 targetToShoot;
} 