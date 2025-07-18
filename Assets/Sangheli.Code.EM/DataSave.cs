using System;
using System.Linq;

[Serializable]
public class DataSave
{
    public int totalEnergy;
    public float timeSinceStart;
    public bool hasVisitedUpgradeMenu;

    public int level;
    
    public ParamContainer paramContainer;
    public EnemyUpgradeContainer enemyUpgradeContainer;
    
    public int upgradesBoughtNum;

    // Сохраняет сгенерированные враги для случайных уровней: ключ - номер уровня, значение - список врагов
    public System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<LevelEnemyInfo>> randomLevelEnemies = new();

    public void SetRandomLevelEnemies(int level, System.Collections.Generic.List<LevelEnemyInfo> enemies)
    {
        if (randomLevelEnemies == null) randomLevelEnemies = new();
        randomLevelEnemies[level] = enemies;
    }

    public System.Collections.Generic.List<LevelEnemyInfo> GetRandomLevelEnemies(int level)
    {
        if (randomLevelEnemies != null && randomLevelEnemies.TryGetValue(level, out var list))
            return list;
        return null;
    }
    
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
            foreach (var enemy in enemyUpgradeContainer.enemyUpgrades)
                if (enemy?.upgradesBought != null)
                    enemyUpgrades += enemy.upgradesBought.Sum();
        }
        upgradesBoughtNum = playerUpgrades + enemyUpgrades;
    }

    public override string ToString()
    {
        return $"{nameof(totalEnergy)}: {totalEnergy}, {nameof(timeSinceStart)}: {timeSinceStart}, {nameof(hasVisitedUpgradeMenu)}: {hasVisitedUpgradeMenu}, {nameof(level)}: {level}, {nameof(upgradesBoughtNum)}: {upgradesBoughtNum}";
    }
}