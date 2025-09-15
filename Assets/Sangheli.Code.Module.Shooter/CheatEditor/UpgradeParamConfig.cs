using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeParamConfig", menuName = "Configs/UpgradeParamConfig")]
public class UpgradeParamConfig : ScriptableObject
{
    public List<ParamInfo> Params = new();
    public ParamInfo GetParamInfo(string key) => Params.FirstOrDefault(x => x.Key == key);
    
    public uint GetTotalPrice()
    {
        // Считаем в ulong, чтобы не было переполнения
        ulong total = Params.Aggregate<ParamInfo, ulong>(0, (current, x) => current + GetSubSum(x));

        if (total > uint.MaxValue)
            throw new OverflowException($"Сумма {total} превысила допустимый предел для uint ({uint.MaxValue}).");

        return (uint)total;
    }

    private static ulong GetSubSum(ParamInfo p)
    {
        if (!p.ShowUpgrade) return 0;
        
        ulong subSum = 0;
        var maxNum = p.MaxUpgrades;
        for (var i = 0; i < maxNum; i++) subSum += p.GetPrice(i, 0);
        return subSum;
    }
}