using System;
using UnityEngine;

public class DamageReciever : MonoBehaviour
{
    public Action<float, bool> Callback;
    public EnemyData EnemyData { get; set; }

    public void TakeDamage(CritResult critResult) => Callback?.Invoke(Mathf.Abs(critResult.Damage), critResult.IsCrit);
}