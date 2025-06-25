using System;

public class UpgradeStateView : BaseUpgradeStateView
{
    private string _paramKey;

    public void Init(string paramKey, Action refreshMoneyCallback = null)
    {
        _paramKey = paramKey;
        RefreshMoneyCallback = refreshMoneyCallback;
        _upgradeButton.onClick.AddListener(OnUpgradeClicked);
        var paramInfo = ParamData.Params[_paramKey];
        MinPrestige = paramInfo.MinPrestige;
    }

    public override void OnUpgradeClicked()
    {
        var save = G.I.SaveProvider.Get();
        var paramContainer = save.paramContainer;
        save.UpdateUpgradesBoughtNum();
        var price = paramContainer.GetPrice(_paramKey, save.upgradesBoughtNum);
        if (!G.I.EnergyManager.SpendEnergyOnUpgrade(price)) 
            return;
        
        paramContainer.BuyUpgrade(_paramKey);
        save.UpdateUpgradesBoughtNum();
        RefreshMoneyCallback?.Invoke();
    }

    public override void Refresh()
    {
        var save = G.I.SaveProvider.Get();
        var paramContainer = save.paramContainer;
        var value = paramContainer.GetValue(_paramKey, true);
        var isMax = paramContainer.IsMaxLevelReached(_paramKey);
        var bought = paramContainer.GetBoughtLevel(_paramKey);
        var max = paramContainer.GetMaxLevel(_paramKey);
        var paramName = paramContainer.GetDisplayName(_paramKey);
        float? nextValue = isMax ? null : paramContainer.GetNextLevelValue(_paramKey);
        save.UpdateUpgradesBoughtNum();
        var price = paramContainer.GetPrice(_paramKey, save.upgradesBoughtNum);
        RefreshView(paramName, value, nextValue, bought, max, isMax, price);
    }
}