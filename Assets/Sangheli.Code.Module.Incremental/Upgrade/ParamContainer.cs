using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class ParamContainer
{
    public Dictionary<string, int> UpgradesBought;

    private List<string> _cachedParamKeys;
    private int _cachedParamKeysCount = -1;
    
    private Dictionary<string, int> TempBought { get; set; } = new();

    public void ResetTemp() => TempBought.Clear();

    public string GetTempBought()
    {
        if (TempBought == null || TempBought.Count == 0)
            return "";
        
        var sb = new StringBuilder();
        foreach (var kvp in TempBought)
        {
            if (sb.Length > 0)
                sb.Append("\n");
            sb.Append($"{kvp.Key}: {kvp.Value}");
        }
        return sb.ToString();
    }

    public void UpdateParamKeysCache()
    {
        var paramList = G.I.Data.ParamData.Params;
        if (_cachedParamKeys != null && _cachedParamKeysCount == paramList.Count) 
            return;
        
        _cachedParamKeys = paramList.Where(x=>x.ShowUpgrade).Select(x => x.Key).ToList();
        _cachedParamKeysCount = paramList.Count;
    }

    public void ValidateAndRepair()
    {
        UpgradesBought ??= new Dictionary<string, int>();
        foreach (var x in G.I.Data.ParamData.Params)
        {
            var key = x.Key;
            
            if (!UpgradesBought.ContainsKey(key))
                UpgradesBought[key] = 0;
            // Ограничиваем уровень максимальным
            int max = x.MaxUpgrades;
            if (UpgradesBought[key] > max)
                UpgradesBought[key] = max;
        }
    }

    /// <summary>
    /// Checks if an upgrade is available for purchase based on dependency requirements
    /// </summary>
    public bool IsUpgradeAvailable(string key)
    {
        var paramInfo = G.I.Data.ParamData.GetParamInfo(key);
        if (paramInfo == null || UpgradesBought[key] >= paramInfo.MaxUpgrades)
            return false;

        // Check fullDepentList - all upgrades in this list must be fully maxed out
        if (paramInfo.fullDepentList != null && paramInfo.fullDepentList.Count > 0)
        {
            foreach (var dependentKey in paramInfo.fullDepentList)
            {
                if (!IsMaxLevelReached(dependentKey))
                    return false;
            }
        }

        // Check anyDepnetList - at least one level must be purchased for each upgrade in this list
        if (paramInfo.anyDepnetList != null && paramInfo.anyDepnetList.Count > 0)
        {
            foreach (var dependentKey in paramInfo.anyDepnetList)
            {
                if (GetBoughtLevel(dependentKey) < 1)
                    return false;
            }
        }

        return true;
    }

    public void BuyUpgrade(string key)
    {
        if (!IsUpgradeAvailable(key))
            return;
        
        UpgradesBought[key]++;

        TempBought.TryAdd(key, 0);
        TempBought[key]++;
    }
    public float GetValue(string key) => G.I.Data.ParamData.GetParamInfo(key)?.GetValue(UpgradesBought[key]) ?? 0;
    public float GetBaseValue(string key) => G.I.Data.ParamData.GetParamInfo(key)?.GetValue( 0) ?? 0;
    public float GetMaxValue(string key) => G.I.Data.ParamData.GetParamInfo(key)?.MaxValue ?? 0;

    public ParamInfo GetCachedParamInfo(string key)
    {
        var level = UpgradesBought[key];
        var value = G.I.Data.ParamData.GetParamInfo(key);
        value.boughtActual = level;
        return value;
    }

    public float GetNextLevelValue(string key)
    {
        var paramInfo = G.I.Data.ParamData.GetParamInfo(key);
        if (paramInfo == null)
            return 0f;
        
        var currentLevel = GetBoughtLevel(key);
        var showLevel = currentLevel >= paramInfo.MaxUpgrades ? currentLevel : currentLevel + 1;
        return paramInfo.GetValue(showLevel);
    }

    public bool IsMaxLevelReached(string key)
    {
        if (!UpgradesBought.TryGetValue(key, out var bought)) 
            return false;

        var info = G.I.Data.ParamData.GetParamInfo(key);
        if (info == null)
            return false;
        
        return bought >= info.MaxUpgrades;
    }

    public int GetBoughtLevel(string key) => UpgradesBought.ContainsKey(key) ? UpgradesBought[key] : 0;
    public int GetMaxLevel(string key) => G.I.Data.ParamData.GetParamInfo(key)?.MaxUpgrades ?? 0;
    public bool GetAutoFormat(string key) => G.I.Data.ParamData.GetParamInfo(key)?.AutoFormat ?? false;
    
    public IEnumerable<string> GetAllParamKeys()
    {
        UpdateParamKeysCache();
        return _cachedParamKeys;
    }
    
    public string GetDisplayName(string key) => G.I.Data.ParamData.GetParamInfo(key)?.DisplayName ?? "---Error---";
    public uint GetPrice(string key, int num) => G.I.Data.ParamData.GetParamInfo(key)?.GetPrice(UpgradesBought[key], num) ?? 0; // Цена за следующий уровень
    public int GetTotalUpgradesBought() => UpgradesBought?.Values.Sum() ?? 0;
}