using System;
using System.Collections.Generic;

[Serializable]
public class EnemyUpgradeData
{
    public int enemyId;
    public List<UpgradeType> upgradeTypes = new();
    public List<int> upgradesBought = new();
        
    public EnemyUpgradeData(int enemyId)
    {
        this.enemyId = enemyId;
        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
            upgradeTypes.Add(type);
            upgradesBought.Add(0);
        }
    }
        
    public int GetLevel(UpgradeType upgradeType)
    {
        var index = upgradeTypes.IndexOf(upgradeType);
        return index >= 0 && index < upgradesBought.Count ? upgradesBought[index] : 0;
    }
        
    public void SetBoughtLevel(UpgradeType upgradeType, int level)
    {
        var index = upgradeTypes.IndexOf(upgradeType);
        if (index >= 0 && index < upgradesBought.Count) 
            upgradesBought[index] = level;
    }
}