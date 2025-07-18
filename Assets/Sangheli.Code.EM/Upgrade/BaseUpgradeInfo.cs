using UnityEngine;

public abstract class BaseUpgradeInfo
{
    public float BaseValue;
    public float MaxValue;
    public int MaxUpgrades => Mathf.FloorToInt((MaxValue - BaseValue) / Step);
    public float Step;
    public string DisplayName;
    public float BasePrice;
    public float PriceMult;
    public bool ShowUpgrade = true;

    public float GetValue(int bought) => Mathf.Max(BaseValue + bought * Step, BaseValue);
    
    public float GetPrice(int bought, int upgradesBoughtNum)
    {
        // var basePrice = BasePrice * Mathf.Pow(PriceMult, bought);
        var basePrice = BasePrice;
        return basePrice * G.I.upgradePriceMultiplierManager.CalculatePriceMultiplier(upgradesBoughtNum);
    }
} 