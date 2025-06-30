using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class ParamContainer
{
    public Dictionary<string, int> UpgradesBought;

    public void ValidateAndRepair()
    {
        UpgradesBought ??= new Dictionary<string, int>();
        foreach (var key in ParamData.Params.Keys)
        {
            if (!UpgradesBought.ContainsKey(key))
                UpgradesBought[key] = 0;
            // Ограничиваем уровень максимальным
            int max = ParamData.Params[key].MaxUpgrades;
            if (UpgradesBought[key] > max)
                UpgradesBought[key] = max;
        }
    }

    public void BuyUpgrade(string key)
    {
        if (UpgradesBought[key] >= ParamData.Params[key].MaxUpgrades) 
            return;
        
        UpgradesBought[key]++;
    }

    public float GetValue(string key) => ParamData.Params[key].GetValue( UpgradesBought[key], false);
    public float GetBaseValue(string key) => ParamData.Params[key].GetValue( UpgradesBought[key], true);

    public float GetNextLevelValue(string key)
    {
        if (!ParamData.Params.TryGetValue(key, out var paramInfo))
            return 0f;
        
        var currentLevel = GetBoughtLevel(key);
        var showLevel = currentLevel >= paramInfo.MaxUpgrades ? currentLevel : currentLevel + 1;
        return paramInfo.GetValue(showLevel, true);
    }

    public bool IsMaxLevelReached(string key)
    {
        if (!UpgradesBought.TryGetValue(key, out var bought)) 
            return false;

        if (!ParamData.Params.TryGetValue(key, out var info)) 
            return false;
        
        return bought >= info.MaxUpgrades;
    }

    public int GetBoughtLevel(string key) => UpgradesBought.ContainsKey(key) ? UpgradesBought[key] : 0;
    public int GetMaxLevel(string key) => ParamData.Params.ContainsKey(key) ? ParamData.Params[key].MaxUpgrades : 0;
    public IEnumerable<string> GetAllParamKeys() => ParamData.Params.Keys;
    public string GetDisplayName(string key) => ParamData.Params[key].DisplayName;
    public float GetPrice(string key, int num) => ParamData.Params[key].GetPrice(UpgradesBought[key], num); // Цена за следующий уровень
    public IEnumerable<string> GetAvailableParamKeys() => ParamData.Params.Keys;
    public int GetTotalUpgradesBought() => UpgradesBought?.Values.Sum() ?? 0;
}