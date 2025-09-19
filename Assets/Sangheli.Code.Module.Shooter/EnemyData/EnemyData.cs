using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Sangheli/EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyController prefab;
    public string DisplayName = "Basic Enemy";
    
    [Space]
    public bool excludeFromLevelGeneration;
    public EEnemyType EnemyType = EEnemyType.Robot;
    public int Tier;
    
    [Space,Header("Sound")]
    public bool playDeathSound;
    public bool playDeathSound2;

    [Space,Header("HP")]
    public bool showHPbar = true;
    public bool cantDie;
    public float BaseHealth;

    [Space,Header("Moving")]
    public EUnitMove UnitMoveType = EUnitMove.Move;
    public float Speed;
    public Vector3 rotationSpeed = new(0, 0, 30);
    
    [Space,Header("Animation")]
    public bool useBounceAnimation = true;
    [FormerlySerializedAs("blockRotation")] public bool blockVisualDirectionChange;
    
    [Space, Header("View scale")]
    public Vector3 viewScale = Vector3.one;
    public Vector3 hitVfx = Vector3.one;
    public Vector3 deathVfxScale = Vector3.one;

    [Space, Header("View tx")] 
    public EnemyDataSprite spriteData;
    
    [Space,Header("Color")]
    public EnemyDataColor colorData;
    
    [Space,Header("Spawn")]
    public EnemyDataSpawn spawnData;

    [Space,Header("Self Destruct")]
    public bool useSelfDestruct;
    
    [Space]
    public bool useRandomSelfDestructTime;
    public float selfDestructTime;
    public float selfDestructTimeRandomAdd;

    [Space] 
    public EnemyDataPos PosConfig;

    [Space,Header("Coins")] 
    public Vector3 SpawnCoinShift;
    
    [Space,Header("Shoot")] 
    public Vector3 shootPoint;
    public float BaseDamage;
    public bool overrideFirstLevel = false;
    public float overrideFirstLevelValue;
    
    [Space,Header("FireRate")]
    public float FireRateMin = 10;
    public bool overrideFireRateFirstDifficult = false;
    public float overrideFireRateFirstDifficultValue = 60;

    [Space,Header("Upgrades")]
    public List<EnemyUpgradeInfo> UpgradeInfos = new()
    {
        new EnemyUpgradeInfo
        {
            Type = UpgradeType.EnergyDrop,
            BaseValue = 1f,
            Step = 0.5f,
            DisplayName = "Min Energy Drop",
            BasePrice = 20,
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
    public uint GetUpgradePrice(UpgradeType upgradeType, int boughtLevel, int upgradesBoughtNum) => GetUpgradeInfo(upgradeType)?.GetPrice(boughtLevel, upgradesBoughtNum) ?? uint.MinValue;
    public int GetMaxUpgradeLevel(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.MaxUpgrades ?? 0;
    public string GetUpgradeDisplayName(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.DisplayName ?? "";
    public virtual float GetEnergy() => GetUpgradeValue(UpgradeType.EnergyDrop, CachedEnergyDropLevel);

    public float GetSelfDestructTime() => useRandomSelfDestructTime ? selfDestructTime + UnityEngine.Random.value * selfDestructTimeRandomAdd : selfDestructTime;

    public void SetBaseHP(float hp) => BaseHealth = hp;
    public void SetBaseDamage(int i) => BaseDamage = i;
    
    public float GetHP(int difficult) => BaseHealth * (difficult+1);
    public float GetDamage(int difficult, int levelIndex)         => overrideFirstLevel && levelIndex < LevelDataConfigRuntime.SkipTutorialLevels ? overrideFirstLevelValue : BaseDamage * (difficult + 1);
    
    public float GetInitTimeAttack(int levelIndex)
    {
        var value = overrideFireRateFirstDifficult && levelIndex < LevelDataConfigRuntime.SkipTutorialLevels ? overrideFireRateFirstDifficultValue : FireRateMin;
        value = 60f/ value + Random.Range(1, 3f);
        return value;
    }

    public float GetTimeAttack(int levelIndex)
    {
        var value = overrideFireRateFirstDifficult && levelIndex < LevelDataConfigRuntime.SkipTutorialLevels ? overrideFireRateFirstDifficultValue : FireRateMin;
        value = 60f / value  + Random.Range(0, 0.5f);
        return value;
    }
}