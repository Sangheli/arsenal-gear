using UnityEngine;

public abstract class BaseUpgradeInfo
{
    public float BaseValue;
    public int MaxUpgrades;
    public float Step;
    public string DisplayName;
    public float BasePrice;
    public float PriceMult;
    public int MinPrestige = 0;

    public virtual float GetValue(int bought, bool onlyDisplay = false) => Mathf.Max(BaseValue + bought * Step, BaseValue);
    
    public virtual float GetPrice(int bought, int upgradesBoughtNum)
    {
        var basePrice = BasePrice * Mathf.Pow(PriceMult, bought);
        return basePrice * Mathf.Pow(G.I.upgradePriceMultiplierManager.GetUpgradesPriceGlobalMultiplier(), upgradesBoughtNum);
    }
} 