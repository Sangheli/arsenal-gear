using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(menuName = "Sangheli/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float Health;
    public float Speed;
    public float DamagePerHit;
    public EnemyController prefab;
    public string DisplayName = "Basic Enemy";
    public bool AttractToCursor = false;
    public float AttractionSpeed = 1.0f;
    public float MinAttractDistance = 2.0f;
    
    public Color color = Color.white;
    public Color borderColor = Color.white;

    public bool superHard;
    
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
    private int CachedEnergyDropLevel { get; set; }

    public void Init(DataSave dataSave)
    {
        var upgradeContainer = dataSave.GetEnemyUpgradeContainer();
        CachedEnergyDropLevel = upgradeContainer.GetBoughtLevel(EnemyId, UpgradeType.EnergyDrop);
    }

    public EnemyUpgradeInfo GetUpgradeInfo(UpgradeType upgradeType) => UpgradeInfos.FirstOrDefault(info => info.Type == upgradeType);
    public float GetUpgradeValue(UpgradeType upgradeType, int boughtLevel) => GetUpgradeInfo(upgradeType)?.GetValue(boughtLevel) ?? 0f;
    public float GeMaxValue(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.MaxValue ?? 0f;
    public float GetUpgradePrice(UpgradeType upgradeType, int boughtLevel, int upgradesBoughtNum) => GetUpgradeInfo(upgradeType)?.GetPrice(boughtLevel, upgradesBoughtNum) ?? float.MaxValue;
    public int GetMaxUpgradeLevel(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.MaxUpgrades ?? 0;
    public string GetUpgradeDisplayName(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.DisplayName ?? "";
    public virtual float GetEnergy() => GetUpgradeValue(UpgradeType.EnergyDrop, CachedEnergyDropLevel);
}