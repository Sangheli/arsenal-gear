using System;

public class UpgradeStateView : BaseUpgradeStateView
{
    private string _paramKey;
    private bool _ignoreHardcoreHide;

    public void Init(string paramKey, Action refreshMoneyCallback = null)
    {
        _paramKey = paramKey;
        _ignoreHardcoreHide = G.I.Data.ParamData.GetParamInfo(_paramKey).ignoreHardcoreHide;
        RefreshMoneyCallback = refreshMoneyCallback;
        _upgradeButton.onClick.AddListener(OnUpgradeClicked);
    }

    public override void OnUpgradeClicked()
    {
        var save = G.I.SaveProvider.Get();
        var paramContainer = save.paramContainer;
        save.UpdateUpgradesBoughtNum();
        var price = paramContainer.GetPrice(_paramKey, save.upgradesBoughtNum);
        if (!G.I.EnergyManager.SpendEnergyOnUpgrade(price)) 
            return;
        
        PlaySound();
        paramContainer.BuyUpgrade(_paramKey);
        save.UpdateUpgradesBoughtNum();
        RefreshMoneyCallback?.Invoke();
    }

    private void PlaySound()
    {
        var pitch = PitchHelper.GetAndIncrement(_paramKey);
        G.I.Container.pooledAudioPlayer.PlayWithPitch(G.I.Data.soundContainer.buttonClick, pitch);
    }

    protected override void UpdateButtonState(bool isMax, float price, bool hardcodeHide)
    {
        var save = G.I.SaveProvider.Get();
        var paramContainer = save.paramContainer;
        var isAvailable = paramContainer.IsUpgradeAvailable(_paramKey);

        if (_ignoreHardcoreHide)
            hardcodeHide = false;
        
        IsDisabled = !(!isMax && G.I.EnergyManager.CanBuyUpgrade(price) && isAvailable) || hardcodeHide;
        _upgradeButton.image.color = GetColor(isMax, IsDisabled);
        _upgradeButton.enabled = GetButtonState(isMax, IsDisabled);
    }

    public override void Refresh(bool hardcodeHide)
    {
        var save = G.I.SaveProvider.Get();
        var paramContainer = save.paramContainer;
        var baseValue = paramContainer.GetBaseValue(_paramKey);
        var value = paramContainer.GetValue(_paramKey);
        var maxValue = paramContainer.GetMaxValue(_paramKey);
        var isMax = paramContainer.IsMaxLevelReached(_paramKey);
        var bought = paramContainer.GetBoughtLevel(_paramKey);
        var max = paramContainer.GetMaxLevel(_paramKey);
        var paramName = G.I.localization.Get(paramContainer.GetDisplayName(_paramKey));
        float? nextValue = isMax ? null : paramContainer.GetNextLevelValue(_paramKey);
        save.UpdateUpgradesBoughtNum();
        var price = paramContainer.GetPrice(_paramKey, save.upgradesBoughtNum);
        RefreshView(paramName, value, nextValue, bought, max, isMax, price, maxValue,baseValue, hardcodeHide);
    }
}