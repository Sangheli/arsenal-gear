using System;
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

    public ulong BasePrice;
    public float PriceMult;
    public bool ShowUpgrade = true;

    public float GetValue(int bought) => Mathf.Max(BaseValue + bought * Step, BaseValue);
    
    public uint GetPrice(int bought, int upgradesBoughtNum)
    {
        var basePrice = BasePrice * Pow(PriceMult, bought);
        if (basePrice > uint.MaxValue)
            throw new OverflowException($"Сумма {basePrice} превысила допустимый предел для uint ({uint.MaxValue}).");
        
        return (uint) basePrice;
        // var basePrice = BasePrice;
        // return basePrice * G.I.upgradePriceMultiplierManager.CalculatePriceMultiplier(upgradesBoughtNum);
    }

    private static ulong Pow(float value, int exp)
    {
        double result = 1;
        for (int i = 0; i < exp; i++) result *= value;
        return (ulong) result;
    }
} 