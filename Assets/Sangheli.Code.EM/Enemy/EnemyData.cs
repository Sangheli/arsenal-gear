using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

[Serializable]
public class EnemyData
{
    public float Health;
    public float Speed;
    public float DamagePerHit;
    public EnemyController prefab;
    public string DisplayName = "Basic Enemy";
    
    public List<EnemyUpgradeInfo> UpgradeInfos = new()
    {
        new EnemyUpgradeInfo
        {
            Type = UpgradeType.EnergyDrop,
            BaseValue = 1f,
            Step = 0.5f,
            DisplayName = "Min Energy Drop",
            BasePrice = 20f,
            PriceMult = 1.2f,
        }
    };
    
    [JsonIgnore]
    public int EnemyId { get; set; }
    private int _cachedEnergyDropLevel;

    public void Init(DataSave dataSave)
    {
        var upgradeContainer = dataSave.GetEnemyUpgradeContainer();
        _cachedEnergyDropLevel = upgradeContainer.GetBoughtLevel(EnemyId, UpgradeType.EnergyDrop);
    }

    public EnemyUpgradeInfo GetUpgradeInfo(UpgradeType upgradeType) => UpgradeInfos.FirstOrDefault(info => info.Type == upgradeType);
    public float GetUpgradeValue(UpgradeType upgradeType, int boughtLevel) => GetUpgradeInfo(upgradeType)?.GetValue(boughtLevel) ?? 0f;
    public float GeMaxValue(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.MaxValue ?? 0f;
    public float GetUpgradePrice(UpgradeType upgradeType, int boughtLevel, int upgradesBoughtNum) => GetUpgradeInfo(upgradeType)?.GetPrice(boughtLevel, upgradesBoughtNum) ?? float.MaxValue;
    public int GetMaxUpgradeLevel(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.MaxUpgrades ?? 0;
    public string GetUpgradeDisplayName(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.DisplayName ?? "";
    public virtual float GetEnergy() => GetUpgradeValue(UpgradeType.EnergyDrop, _cachedEnergyDropLevel);
}