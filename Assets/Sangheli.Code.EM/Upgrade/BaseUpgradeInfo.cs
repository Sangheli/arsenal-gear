using UnityEngine;

public abstract class BaseUpgradeInfo
{
    public int row = 0;
    [Space]
    public string DisplayName;
    [Space]
    public float BaseValue;
    public float MaxValue;
    public int MaxUpgrades => Mathf.FloorToInt((MaxValue - BaseValue) / Step);
    public float Step;

    public float BasePrice;
    public float PriceMult;
    public bool ShowUpgrade = true;

    public float GetValue(int bought) => Mathf.Max(BaseValue + bought * Step, BaseValue);
    
    public float GetPrice(int bought, int upgradesBoughtNum)
    {
        var basePrice = BasePrice * Mathf.Pow(PriceMult, bought);
        return basePrice;
        // var basePrice = BasePrice;
        // return basePrice * G.I.upgradePriceMultiplierManager.CalculatePriceMultiplier(upgradesBoughtNum);
    }
} 