using System;
using UnityEngine;

public class DamageReciever : MonoBehaviour
{
    public Action<float, bool> Callback;
    public EnemyData EnemyData { get; set; }

    public void TakeDamage(float dmg)
    {
        bool isCrit = dmg < 0;
        Callback?.Invoke(Mathf.Abs(dmg), isCrit);
    }
}