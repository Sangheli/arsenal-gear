using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class EnemyData
{
    public float Health;
    public float Speed;
    public float DamagePerHit;
    public EnemyView prefab;
    public int EnemyId { get; set; }
    public string DisplayName = "Basic Enemy";
    
    public List<EnemyUpgradeInfo> UpgradeInfos = new()
    {
        new EnemyUpgradeInfo
        {
            Type = UpgradeType.EnergyDrop,
            BaseValue = 1f,
            MaxUpgrades = 15,
            Step = 0.5f,
            DisplayName = "Min Energy Drop",
            BasePrice = 20f,
            PriceMult = 1.2f,
        },
        new EnemyUpgradeInfo
        {
            Type = UpgradeType.SpawnRate,
            BaseValue = 10f,
            MaxUpgrades = 8,
            Step = -1f,
            DisplayName = "Spawn Rate (lower = faster)",
            BasePrice = 40f,
            PriceMult = 1.2f,
        }
    };
    
    private int _cachedEnergyDropLevel;
    private int _cachedSpawnRateLevel;

    public void Init(DataSave dataSave)
    {
        var upgradeContainer = dataSave.GetEnemyUpgradeContainer();
        _cachedEnergyDropLevel = upgradeContainer.GetBoughtLevel(EnemyId, UpgradeType.EnergyDrop);
        _cachedSpawnRateLevel = upgradeContainer.GetBoughtLevel(EnemyId, UpgradeType.SpawnRate);
    }

    public EnemyUpgradeInfo GetUpgradeInfo(UpgradeType upgradeType) => UpgradeInfos.FirstOrDefault(info => info.Type == upgradeType);
    public float GetUpgradeValue(UpgradeType upgradeType, int boughtLevel) => GetUpgradeInfo(upgradeType)?.GetValue(boughtLevel) ?? 0f;
    public float GetUpgradePrice(UpgradeType upgradeType, int boughtLevel, int upgradesBoughtNum) => GetUpgradeInfo(upgradeType)?.GetPrice(boughtLevel, upgradesBoughtNum) ?? float.MaxValue;
    public int GetMaxUpgradeLevel(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.MaxUpgrades ?? 0;
    public string GetUpgradeDisplayName(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.DisplayName ?? "";
    public virtual float GetEnergy() => GetUpgradeValue(UpgradeType.EnergyDrop, _cachedEnergyDropLevel);
    public virtual float GetSpawnRatePerMinute() => GetUpgradeValue(UpgradeType.SpawnRate, _cachedSpawnRateLevel);
}