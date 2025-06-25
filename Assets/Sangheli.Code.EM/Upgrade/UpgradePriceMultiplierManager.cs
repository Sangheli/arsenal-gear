using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class UpgradePriceMultiplierManager
{
    public float minMultiplier = 1.0f;
    public float maxMultiplier = 1.2f;
    public AnimationCurve multiplierCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public float GetUpgradesPriceGlobalMultiplier()
    {
        var save = G.I.SaveProvider.Get();
        var maxUpgrades = GetTotalPossibleUpgrades();
        var t = maxUpgrades > 0 ? Mathf.Clamp01((float)save.upgradesBoughtNum / maxUpgrades) : 0f;
        return Mathf.Lerp(minMultiplier, maxMultiplier, multiplierCurve.Evaluate(t));
    }
    
    public int GetTotalPossibleUpgrades()
    {
        var playerMax = ParamData.Params.Values.Sum(param => param.MaxUpgrades);
        var enemyMax = GetEnemyMax();
        return playerMax + enemyMax;
    }

    private int GetEnemyMax()
    {
        var enemyMax = 0;
        var enemyContainer = G.I.enemyContainer;
        if (enemyContainer != null && enemyContainer.List != null)
            enemyMax += enemyContainer.List
                .Where(enemy => enemy is { UpgradeInfos: { } })
                .Sum(enemy => enemy.UpgradeInfos.Sum(info => info.MaxUpgrades));
        
        return enemyMax;
    }
} 