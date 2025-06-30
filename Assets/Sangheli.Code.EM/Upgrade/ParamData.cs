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
                PriceMult = 1.08f,
                UseRandom = true,
                RandomCoef = 1.5f
            }
        },
        {
            "hp", new ParamInfo
            {
                BaseValue = 10,
                MaxUpgrades = 30,
                Step = 10f,
                DisplayName = "HP",
                BasePrice = 3,
                PriceMult = 1.07f
            }
        },
        {
            "playerFireRate", new ParamInfo
            {
                BaseValue = 60f,
                MaxUpgrades = 60,
                Step = 5f,
                DisplayName = "Attacks/minute",
                BasePrice = 2,
                PriceMult = 1.06f
            }
        },
        {
            "damageDiameter", new ParamInfo
            {
                BaseValue = 1f,
                MaxUpgrades = 40,
                Step = 0.2f,
                DisplayName = "Diameter",
                BasePrice = 3,
                PriceMult = 1.07f
            }
        },
        {
            "collectDiameter", new ParamInfo
            {
                BaseValue = 0.25f,
                MaxUpgrades = 50,
                Step = 0.3f,
                DisplayName = "Collect Additional Diameter",
                BasePrice = 4,
                PriceMult = 1.08f
            }
        },
        {
            "shieldCapacity", new ParamInfo
            {
                BaseValue = 0f,
                MaxUpgrades = 25,
                Step = 1f,
                DisplayName = "Shield Capacity",
                BasePrice = 5,
                PriceMult = 1.09f
            }
        },
        {
            "shieldRegenRate", new ParamInfo
            {
                BaseValue = 0f,
                MaxUpgrades = 50,
                Step = 0.5f,
                DisplayName = "Shield Regen/sec",
                BasePrice = 500,
                PriceMult = 1.2f
            }
        },
        {
            "critChance", new ParamInfo
            {
                BaseValue = 0f,
                MaxUpgrades = 40,
                Step = 2.5f,
                DisplayName = "Crit Chance %",
                BasePrice = 1000,
                PriceMult = 1.30f
            }
        },
        {
            "critMultiplier", new ParamInfo
            {
                BaseValue = 1f,
                MaxUpgrades = 10,
                Step = 0.2f,
                DisplayName = "Crit Multiplier",
                BasePrice = 7,
                PriceMult = 1.12f
            }
        },
        {
            "autoCollectCoinsPercent", new ParamInfo
            {
                BaseValue = 0f,
                MaxUpgrades = 25,
                Step = 2f,
                DisplayName = "Auto Collect Coins %",
                BasePrice = 1000,
                PriceMult = 1.3f
            }
        },
        {
            "aiCursorCount", new ParamInfo
            {
                BaseValue = 0,
                MaxUpgrades = 5,
                Step = 1f,
                DisplayName = "AI Cursor Count",
                BasePrice = 100,
                PriceMult = 1.15f
            }
        },
        {
            "aiCursorDamagePercent", new ParamInfo
            {
                BaseValue = 20f,
                MaxUpgrades = 8,
                Step = 10f,
                DisplayName = "AI Cursor Damage % of Player",
                BasePrice = 200,
                PriceMult = 1.13f
            }
        },
        {
            "aiCursorRadiusPercent", new ParamInfo
            {
                BaseValue = 50f,
                MaxUpgrades = 10,
                Step = 5f,
                DisplayName = "AI Cursor Radius % of Player",
                BasePrice = 300,
                PriceMult = 1.12f
            }
        },
        {
            "aiCursorFireRatePercent", new ParamInfo
            {
                BaseValue = 20f,
                MaxUpgrades = 8,
                Step = 10f,
                DisplayName = "AI Cursor Fire Rate % of Player",
                BasePrice = 400,
                PriceMult = 1.13f
            }
        },
    };
}