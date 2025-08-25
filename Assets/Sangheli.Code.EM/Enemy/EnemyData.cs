using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(menuName = "Sangheli/EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyController prefab;
    public string DisplayName = "Basic Enemy";
    
    [Space]
    public bool excludeFromLevelGeneration;
    public bool superHard;
    public bool playDeathSound;
    
    [Space]
    public float Health;
    public float DamagePerHit;
    public float Speed;
    public Vector3 rotationSpeed = new(0, 0, 30);
    
    [Space, Header("View scale")]
    public Vector3 viewScale = Vector3.one;
    public Vector3 deathVfxScale = Vector3.one;

    [Space, Header("View tx")] 
    public Sprite spriteView;
    public Sprite spriteBorder;
    
    [Space,Header("Magnet")]
    public bool AttractToCursor = false;
    public float AttractionSpeed = 1.0f;
    public float MinAttractDistance = 2.0f;
    
    [Space,Header("Color")]
    public bool useRandomColor;
    
    [Space]
    public Color color = Color.white;
    public Color borderColor = Color.white;
    
    [Space,Header("Spaw Rate")]
    public bool useCustomSpawnRate;
    
    [Space]
    public bool useRandomSpawnRate;
    public int spawnRate1;
    public int spawnRate2;

    [Space,Header("Self Destruct")]
    public bool useSelfDestruct;
    
    [Space]
    public bool useRandomSelfDestructTime;
    public float selfDestructTime;
    public float selfDestructTimeRandomAdd;

    [Space] 
    public int Tier;
    
    [Space,Header("Upgrades")]
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
    
    public int GetSpawnRate() => useRandomSpawnRate ? Random.Range(spawnRate1, spawnRate2) : spawnRate1;

    public EnemyUpgradeInfo GetUpgradeInfo(UpgradeType upgradeType) => UpgradeInfos.FirstOrDefault(info => info.Type == upgradeType);
    public float GetUpgradeValue(UpgradeType upgradeType, int boughtLevel) => GetUpgradeInfo(upgradeType)?.GetValue(boughtLevel) ?? 0f;
    public float GeMaxValue(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.MaxValue ?? 0f;
    public float GetUpgradePrice(UpgradeType upgradeType, int boughtLevel, int upgradesBoughtNum) => GetUpgradeInfo(upgradeType)?.GetPrice(boughtLevel, upgradesBoughtNum) ?? float.MaxValue;
    public int GetMaxUpgradeLevel(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.MaxUpgrades ?? 0;
    public string GetUpgradeDisplayName(UpgradeType upgradeType) => GetUpgradeInfo(upgradeType)?.DisplayName ?? "";
    public virtual float GetEnergy() => GetUpgradeValue(UpgradeType.EnergyDrop, CachedEnergyDropLevel);

    public Color GetColor() => useRandomColor ? GetRandomColor() : color;
    public Color GetBorderColor() => useRandomColor ? GetRandomColor() : borderColor;
    private Color GetRandomColor()
    {
        // Generate vibrant colors that fit the neon aesthetic of the game
        float hue = UnityEngine.Random.value;
        float saturation = 0.8f + UnityEngine.Random.value * 0.2f; // 0.8-1.0 for vibrant colors
        float value = 0.8f + UnityEngine.Random.value * 0.2f; // 0.8-1.0 for bright colors
        
        var color = Color.HSVToRGB(hue, saturation, value);
        color.a = 1;

        return color;
    }

    public float GetSelfDestructTime() => useRandomSelfDestructTime ? selfDestructTime + UnityEngine.Random.value * selfDestructTimeRandomAdd : selfDestructTime;
}