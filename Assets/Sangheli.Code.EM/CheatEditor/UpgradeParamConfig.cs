using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeParamConfig", menuName = "Configs/UpgradeParamConfig")]
public class UpgradeParamConfig : ScriptableObject
{
    public List<ParamInfo> Params = new();
    public ParamInfo GetParamInfo(string key) => Params.FirstOrDefault(x => x.Key == key);
    
    public int GetTotalPrice() => Params.Where(p => p.ShowUpgrade).Sum(GetSubSum);

    private static int GetSubSum(ParamInfo p)
    {
        var subSum = 0;
        var maxNum = p.MaxUpgrades;
        for (var i = 0; i < maxNum; i++) subSum += p.GetPrice(i, 0);
        return subSum;
    }
}