using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class EnemyUpgradeContainer
{
    public List<EnemyUpgradeData> enemyUpgrades;

    public void ValidateAndRepair()
    {
        enemyUpgrades ??= new List<EnemyUpgradeData>();
        
        // Получаем все актуальные enemyId из EnemyContainer
        var allEnemyIds = G.I.enemyContainer.List
            .Select(x => x.EnemyId).ToList();

        // Добавляем недостающие EnemyUpgradeData
        foreach (var enemyId in allEnemyIds.Where(enemyId => enemyUpgrades.All(e => e.enemyId != enemyId)))
            enemyUpgrades.Add(new EnemyUpgradeData(enemyId));
        
        // Удаляем устаревшие EnemyUpgradeData
        enemyUpgrades.RemoveAll(e => !allEnemyIds.Contains(e.enemyId));
        
        // Проверяем и чиним upgradeTypes и upgradesBought для каждого EnemyUpgradeData
        foreach (var enemyUpgrade in enemyUpgrades)
        {
            var allTypes = Enum.GetValues(typeof(UpgradeType));
            // Добавляем недостающие типы
            foreach (UpgradeType type in allTypes)
            {
                if (enemyUpgrade.upgradeTypes.Contains(type)) 
                    continue;
                
                enemyUpgrade.upgradeTypes.Add(type);
                enemyUpgrade.upgradesBought.Add(0);
            }
            
            // Удаляем лишние типы
            for (var i = enemyUpgrade.upgradeTypes.Count - 1; i >= 0; i--)
            {
                if (Enum.IsDefined(typeof(UpgradeType), enemyUpgrade.upgradeTypes[i])) 
                    continue;
                
                enemyUpgrade.upgradeTypes.RemoveAt(i);
                enemyUpgrade.upgradesBought.RemoveAt(i);
            }
            
            // Синхронизируем длины списков
            while (enemyUpgrade.upgradesBought.Count < enemyUpgrade.upgradeTypes.Count)
                enemyUpgrade.upgradesBought.Add(0);
            
            while (enemyUpgrade.upgradesBought.Count > enemyUpgrade.upgradeTypes.Count)
                enemyUpgrade.upgradesBought.RemoveAt(enemyUpgrade.upgradesBought.Count - 1);
            
            // Ограничиваем уровни апгрейдов максимальными
            var enemyData = G.I.enemyContainer.List.Find(e => e.EnemyId == enemyUpgrade.enemyId);
            if (enemyData != null)
            {
                for (int i = 0; i < enemyUpgrade.upgradeTypes.Count; i++)
                {
                    var type = enemyUpgrade.upgradeTypes[i];
                    var info = enemyData.UpgradeInfos.Find(x => x.Type == type);
                    if (info != null && enemyUpgrade.upgradesBought[i] > info.MaxUpgrades)
                        enemyUpgrade.upgradesBought[i] = info.MaxUpgrades;
                }
            }
        }
    }

    public void Init(List<EnemyData> List)
    {
        enemyUpgrades ??= List
            .GroupBy(x => x.EnemyId)
            .Select(g => new EnemyUpgradeData(g.Key))
            .ToList();
    }

    public void BuyUpgrade(int enemyId, UpgradeType upgradeType)
    {
        var enemyData = GetEnemyData(enemyId);
        var currentLevel = enemyData.GetLevel(upgradeType);
        var maxLevel = GetMaxLevel(enemyId, upgradeType);
        if (currentLevel < maxLevel) enemyData.SetBoughtLevel(upgradeType, currentLevel + 1);
    }

    public float GetValue(int enemyId, UpgradeType upgradeType)
    {
        var boughtLevel = GetEnemyData(enemyId).GetLevel(upgradeType);
        return GetUpgradeValueFromEnemyData(enemyId, upgradeType, boughtLevel);
    }

    public float GetNextLevelValue(int enemyId, UpgradeType upgradeType)
    {
        var currentLevel = GetEnemyData(enemyId).GetLevel(upgradeType);
        var maxLevel = GetMaxLevel(enemyId, upgradeType);

        var level = currentLevel >= maxLevel ? currentLevel : currentLevel + 1;
        return GetUpgradeValueFromEnemyData(enemyId, upgradeType, level);
    }

    private EnemyUpgradeData GetEnemyData(int enemyId)
    {
        var enemyData = enemyUpgrades.Find(e => e.enemyId == G.I.enemyContainer.List[enemyId].EnemyId);
        if (enemyData != null) 
            return enemyData;
        
        enemyData = new EnemyUpgradeData(G.I.enemyContainer.List[enemyId].EnemyId);
        enemyUpgrades.Add(enemyData);
        return enemyData;
    }

    public bool IsMaxLevelReached(int enemyId, UpgradeType upgradeType)
    {
        var info = GetUpgradeInfoFromEnemyData(enemyId, upgradeType);
        return info == null || GetEnemyData(enemyId).GetLevel(upgradeType) >= info.MaxUpgrades;
    }

    public int GetBoughtLevel(int id, UpgradeType type) => GetEnemyData(id)?.GetLevel(type) ?? 0;

    public int GetMaxLevel(int enemyId, UpgradeType upgradeType) => 
        GetUpgradeInfoFromEnemyData(enemyId, upgradeType)?.MaxUpgrades ?? 0;

    public string GetDisplayName(int enemyId, UpgradeType upgradeType) => 
        GetUpgradeInfoFromEnemyData(enemyId, upgradeType)?.DisplayName ?? "";

    public string GetEnemyDisplayName(int enemyId) => GetEnemyDataById(enemyId)?.DisplayName;

    public int GetTotalUpgradesBought()
    {
        if (enemyUpgrades == null) return 0;
        int sum = 0;
        foreach (var enemy in enemyUpgrades)
            if (enemy?.upgradesBought != null)
                sum += enemy.upgradesBought.Sum();
        return sum;
    }

    public int GetPrice(int enemyId, UpgradeType upgradeType, int upgradesBoughtNum)
    {
        var boughtLevel = GetEnemyData(enemyId).GetLevel(upgradeType);
        return (int) GetUpgradePriceFromEnemyData(enemyId, upgradeType, boughtLevel, upgradesBoughtNum);
    }

    public IEnumerable<int> GetAllEnemyIds() => Enumerable.Range(0, G.I.enemyContainer.List.Count);

    public IEnumerable<UpgradeType> GetAllUpgradeTypes(int enemyId)
    {
        var enemyData = GetEnemyDataById(enemyId);
        return enemyData == null 
            ? new List<UpgradeType>() 
            : Enum.GetValues(typeof(UpgradeType)).Cast<UpgradeType>().Where(x=>x!=UpgradeType.None);
    }

    private EnemyData GetEnemyDataById(int enemyId) => 
        G.I.enemyContainer.List.Find(e => e.EnemyId == G.I.enemyContainer.List[enemyId].EnemyId);

    private EnemyUpgradeInfo GetUpgradeInfoFromEnemyData(int enemyId, UpgradeType upgradeType) => 
        GetEnemyDataById(enemyId)?.GetUpgradeInfo(upgradeType);

    private float GetUpgradeValueFromEnemyData(int enemyId, UpgradeType upgradeType, int boughtLevel) => 
        GetEnemyDataById(enemyId)?.GetUpgradeValue(upgradeType, boughtLevel) ?? 0f;

    private float GetUpgradePriceFromEnemyData(int enemyId, UpgradeType upgradeType, int boughtLevel, int upgradesBoughtNum) => 
        GetEnemyDataById(enemyId)?.GetUpgradePrice(upgradeType, boughtLevel, upgradesBoughtNum) ?? 0;

    public IEnumerable<UpgradeType> GetAvailableUpgradeTypes(int enemyId, int prestige)
    {
        var enemyData = GetEnemyDataById(enemyId);
        if (enemyData == null) return new List<UpgradeType>();
        return enemyData.UpgradeInfos
            .Select(info => info.Type);
    }
} 