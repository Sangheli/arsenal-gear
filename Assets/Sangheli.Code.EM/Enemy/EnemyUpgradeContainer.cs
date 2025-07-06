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
        foreach (var x in allEnemyIds.Where(x => enemyUpgrades.All(e => e.enemyId != x)))
            enemyUpgrades.Add(new EnemyUpgradeData(x));
        
        // Удаляем устаревшие EnemyUpgradeData
        enemyUpgrades.RemoveAll(e => !allEnemyIds.Contains(e.enemyId));
        
        // Проверяем и чиним upgradeTypes и upgradesBought для каждого EnemyUpgradeData
        foreach (var x in enemyUpgrades) UpdateData(x);
    }

    private static void UpdateData(EnemyUpgradeData item)
    {
        var allTypes = Enum.GetValues(typeof(UpgradeType));
        // Добавляем недостающие типы
        foreach (UpgradeType type in allTypes)
        {
            if (item.upgradeTypes.Contains(type))
                continue;

            item.upgradeTypes.Add(type);
            item.upgradesBought.Add(0);
        }

        // Удаляем лишние типы
        for (var i = item.upgradeTypes.Count - 1; i >= 0; i--)
        {
            if (Enum.IsDefined(typeof(UpgradeType), item.upgradeTypes[i]))
                continue;

            item.upgradeTypes.RemoveAt(i);
            item.upgradesBought.RemoveAt(i);
        }

        // Синхронизируем длины списков
        while (item.upgradesBought.Count < item.upgradeTypes.Count)
            item.upgradesBought.Add(0);

        while (item.upgradesBought.Count > item.upgradeTypes.Count)
            item.upgradesBought.RemoveAt(item.upgradesBought.Count - 1);

        // Ограничиваем уровни апгрейдов максимальными
        var enemyData = G.I.enemyContainer.List.Find(e => e.EnemyId == item.enemyId);
        if (enemyData == null)
            return;

        for (int i = 0; i < item.upgradeTypes.Count; i++)
        {
            var type = item.upgradeTypes[i];
            var info = enemyData.UpgradeInfos.Find(x => x.Type == type);
            if (info != null && item.upgradesBought[i] > info.MaxUpgrades)
                item.upgradesBought[i] = info.MaxUpgrades;
        }
    }

    public void Init(List<EnemyData> list)
    {
        enemyUpgrades ??= list
            .GroupBy(x => x.EnemyId)
            .Select(g => new EnemyUpgradeData(g.Key))
            .ToList();
    }

    public void BuyUpgrade(int id, UpgradeType type)
    {
        var enemyData = GetEnemyData(id);
        var currentLevel = enemyData.GetLevel(type);
        var maxLevel = GetMaxLevel(id, type);
        if (currentLevel < maxLevel) enemyData.SetBoughtLevel(type, currentLevel + 1);
    }

    public float GetValue(int id, UpgradeType type) => GetUpgradeValueFromEnemyData(id, type,  GetEnemyData(id).GetLevel(type));
    public float GetBaseValue(int id, UpgradeType type) => GetUpgradeValueFromEnemyData(id, type,  0);
    public float GetMaxValue(int enemyId, UpgradeType upgradeType)
    {
        var level = GetEnemyData(enemyId).GetLevel(upgradeType);
        return GetById(enemyId)?.GeMaxValue(upgradeType) ?? 0f;
    }

    public float GetNextLevelValue(int id, UpgradeType type)
    {
        var currentLevel = GetEnemyData(id).GetLevel(type);
        var maxLevel = GetMaxLevel(id, type);

        var level = currentLevel >= maxLevel ? currentLevel : currentLevel + 1;
        return GetUpgradeValueFromEnemyData(id, type, level);
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
    public int GetMaxLevel(int id, UpgradeType type) => GetUpgradeInfoFromEnemyData(id, type)?.MaxUpgrades ?? 0;

    public string GetDisplayName(int id, UpgradeType type) => GetUpgradeInfoFromEnemyData(id, type)?.DisplayName ?? "";

    public string GetEnemyDisplayName(int enemyId) => GetById(enemyId)?.DisplayName;

    public int GetTotalUpgradesBought() => 
        enemyUpgrades != null 
            ? enemyUpgrades.Where(enemy => enemy?.upgradesBought != null).Sum(enemy => enemy.upgradesBought.Sum()) 
            : 0;

    public int GetPrice(int enemyId, UpgradeType upgradeType, int upgradesBoughtNum)
    {
        var boughtLevel = GetEnemyData(enemyId).GetLevel(upgradeType);
        return (int) GetUpgradePriceFromEnemyData(enemyId, upgradeType, boughtLevel, upgradesBoughtNum);
    }

    public IEnumerable<int> GetAllEnemyIds() => Enumerable.Range(0, G.I.enemyContainer.List.Count);

    public IEnumerable<UpgradeType> GetAllUpgradeTypes(int id)
    {
        var enemyData = GetById(id);
        return enemyData == null 
            ? new List<UpgradeType>() 
            : Enum.GetValues(typeof(UpgradeType)).Cast<UpgradeType>().Where(x=>x!=UpgradeType.None);
    }

    private EnemyData GetById(int id) => G.I.enemyContainer.List.Find(e => e.EnemyId == G.I.enemyContainer.List[id].EnemyId);
    private EnemyUpgradeInfo GetUpgradeInfoFromEnemyData(int id, UpgradeType type) => GetById(id)?.GetUpgradeInfo(type);
    private float GetUpgradeValueFromEnemyData(int id, UpgradeType type, int level) => GetById(id)?.GetUpgradeValue(type, level) ?? 0f;
    private float GetUpgradePriceFromEnemyData(int id, UpgradeType type, int level, int num) => GetById(id)?.GetUpgradePrice(type, level, num) ?? 0;
    public IEnumerable<UpgradeType> GetAvailableUpgradeTypes(int id) => GetById(id)?.UpgradeInfos.Select(info => info.Type) ?? new List<UpgradeType>();
} 