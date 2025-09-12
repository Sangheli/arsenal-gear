using System;
using System.Linq;

[Serializable]
public class DataSave
{
    public int totalEnergy;
    public float timeSinceStart;
    public bool hasVisitedUpgradeMenu;
    public int level;
    public int loadTimes;
    
    public int saveVersion = 0;
    
    public int wins;
    public int losses;
    public int upgradesBoughtNum;
    public int shopVisits;

    public ParamContainer paramContainer;
    public EnemyUpgradeContainer enemyUpgradeContainer;
    
    public ParamContainer GetParamContainer() => paramContainer ??= new ParamContainer();
    public EnemyUpgradeContainer GetEnemyUpgradeContainer() => enemyUpgradeContainer ??= new EnemyUpgradeContainer();

    public void Validate()
    {
        GetParamContainer().ValidateAndRepair();
        GetEnemyUpgradeContainer().ValidateAndRepair();
    }

    public void UpdateUpgradesBoughtNum()
    {
        var playerUpgrades = paramContainer?.UpgradesBought?.Values.Sum() ?? 0;
        var enemyUpgrades = 0;
        
        if (enemyUpgradeContainer?.enemyUpgrades != null)
        {
            enemyUpgrades += enemyUpgradeContainer
                .enemyUpgrades
                .Where(enemy => enemy?.upgradesBought != null)
                .Sum(enemy => enemy.upgradesBought.Sum());
        }
        
        upgradesBoughtNum = playerUpgrades + enemyUpgrades;
    }

    public override string ToString()
    {
        return $"{nameof(totalEnergy)}: {totalEnergy}, {nameof(timeSinceStart)}: {timeSinceStart}, {nameof(hasVisitedUpgradeMenu)}: {hasVisitedUpgradeMenu}, {nameof(level)}: {level}, {nameof(upgradesBoughtNum)}: {upgradesBoughtNum}, {nameof(wins)}: {wins}, {nameof(losses)}: {losses}, {nameof(shopVisits)}: {shopVisits}, {nameof(saveVersion)}: {saveVersion}";
    }
}