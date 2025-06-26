using System.Collections.Generic;

public class ParamData
{
    public static readonly Dictionary<string, ParamInfo> Params = new()
    {
        {
            "playerDamageMin", new ParamInfo
            {
                BaseValue = 5f,
                MaxUpgrades = 20,
                Step = 1f,
                DisplayName = "Damage",
                BasePrice = 2,
                PriceMult = 1.20f,
                UseRandom = true,
                RandomCoef = 1.5f
            }
        },
        {
            "hp", new ParamInfo
            {
                BaseValue = 10,
                MaxUpgrades = 30,
                Step = 2f,
                DisplayName = "HP",
                BasePrice = 5,
                PriceMult = 1.2f
            }
        },
        {
            "playerFireRate", new ParamInfo
            {
                BaseValue = 60f,
                MaxUpgrades = 60,
                Step = 5f,
                DisplayName = "Attacks/minute",
                BasePrice = 4,
                PriceMult = 1.2f
            }
        },
        {
            "damageDiameter", new ParamInfo
            {
                BaseValue = 1f,
                MaxUpgrades = 40,
                Step = 0.05f,
                DisplayName = "Diameter",
                BasePrice = 5,
                PriceMult = 1.2f
            }
        },
        {
            "collectDiameter", new ParamInfo
            {
                BaseValue = 0.25f,
                MaxUpgrades = 50,
                Step = 0.3f,
                DisplayName = "Collect Additional Diameter",
                BasePrice = 10,
                PriceMult = 1.2f
            }
        },
        {
            "shieldCapacity", new ParamInfo
            {
                BaseValue = 2f,
                MaxUpgrades = 15,
                Step = 1f,
                DisplayName = "Shield Capacity",
                BasePrice = 40,
                PriceMult = 1.2f
            }
        },
        {
            "shieldRegenRate", new ParamInfo
            {
                BaseValue = 0.1f,
                MaxUpgrades = 50,
                Step = 0.1f,
                DisplayName = "Shield Regen/sec",
                BasePrice = 45,
                PriceMult = 1.2f
            }
        },
        {
            "critChance", new ParamInfo
            {
                BaseValue = 5f,
                MaxUpgrades = 38,
                Step = 2.5f,
                DisplayName = "Crit Chance %",
                BasePrice = 30,
                PriceMult = 1.25f
            }
        },
        {
            "critMultiplier", new ParamInfo
            {
                BaseValue = 1f,
                MaxUpgrades = 10,
                Step = 0.2f,
                DisplayName = "Crit Multiplier",
                BasePrice = 40,
                PriceMult = 1.3f
            }
        },
        {
            "critTargetsCount", new ParamInfo
            {
                BaseValue = 1f,
                MaxUpgrades = 10,
                Step = 1f,
                DisplayName = "Crit Targets Count",
                BasePrice = 50,
                PriceMult = 1.3f
            }
        },
        {
            "autoCollectCoinsPercent", new ParamInfo
            {
                BaseValue = 0f,
                MaxUpgrades = 20,
                Step = 5f,
                DisplayName = "Auto Collect Coins %",
                BasePrice = 20,
                PriceMult = 1.25f
            }
        },
        {
            "aiCursorCount", new ParamInfo
            {
                BaseValue = 0,
                MaxUpgrades = 5,
                Step = 1f,
                DisplayName = "AI Cursor Count",
                BasePrice = 1000,
                PriceMult = 3.0f
            }
        },
        {
            "aiCursorDamagePercent", new ParamInfo
            {
                BaseValue = 0.2f,
                MaxUpgrades = 10,
                Step = 0.08f,
                DisplayName = "AI Cursor Damage % of Player",
                BasePrice = 2000,
                PriceMult = 2.5f
            }
        },
        {
            "aiCursorRadiusPercent", new ParamInfo
            {
                BaseValue = 0.5f,
                MaxUpgrades = 10,
                Step = 0.05f,
                DisplayName = "AI Cursor Radius % of Player",
                BasePrice = 1500,
                PriceMult = 2.2f
            }
        },
        {
            "aiCursorFireRatePercent", new ParamInfo
            {
                BaseValue = 0.3f,
                MaxUpgrades = 10,
                Step = 0.07f,
                DisplayName = "AI Cursor Fire Rate % of Player",
                BasePrice = 1800,
                PriceMult = 2.3f
            }
        },
    };
}