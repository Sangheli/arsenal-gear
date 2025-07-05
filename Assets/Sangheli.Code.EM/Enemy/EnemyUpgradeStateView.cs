using System;

public class EnemyUpgradeStateView : BaseUpgradeStateView
{
    private int _enemyId;
    private UpgradeType _upgradeType;

    public void Init(int enemyId, UpgradeType upgradeType, Action refreshMoneyCallback = null)
    {
        _enemyId = enemyId;
        _upgradeType = upgradeType;
        RefreshMoneyCallback = refreshMoneyCallback;
        _upgradeButton.onClick.AddListener(OnUpgradeClicked);
    }

    public override void OnUpgradeClicked()
    {
        var save = G.I.SaveProvider.Get();
        var upgradeContainer = save.GetEnemyUpgradeContainer();
        save.UpdateUpgradesBoughtNum();
        var price = upgradeContainer.GetPrice(_enemyId, _upgradeType, save.upgradesBoughtNum);
        if (!G.I.EnergyManager.SpendEnergyOnUpgrade(price)) 
            return;
        
        PlayAudio();
        upgradeContainer.BuyUpgrade(_enemyId, _upgradeType);
        save.UpdateUpgradesBoughtNum();
        RefreshMoneyCallback?.Invoke();
    }

    private void PlayAudio()
    {
        // Логика pitch
        string key = $"enemy_{_enemyId}_{_upgradeType}";
        float pitch = UpgradePitchHelper.GetAndIncrement(key);
        G.I.Container.pooledAudioPlayer.PlayWithPitch(G.I.Data.soundContainer.buttonClick, pitch);
    }

    public override void Refresh()
    {
        var save = G.I.SaveProvider.Get();
        var upgradeContainer = save.GetEnemyUpgradeContainer();
        var value = upgradeContainer.GetValue(_enemyId, _upgradeType);
        var maxValue = upgradeContainer.GetMaxValue(_enemyId, _upgradeType);
        var isMax = upgradeContainer.IsMaxLevelReached(_enemyId, _upgradeType);
        var bought = upgradeContainer.GetBoughtLevel(_enemyId, _upgradeType);
        var max = upgradeContainer.GetMaxLevel(_enemyId, _upgradeType);
        var paramName = $"[{ upgradeContainer.GetEnemyDisplayName(_enemyId)}] {upgradeContainer.GetDisplayName(_enemyId, _upgradeType)}";
        float? nextValue = isMax ? null : upgradeContainer.GetNextLevelValue(_enemyId, _upgradeType);
        save.UpdateUpgradesBoughtNum();
        var price = upgradeContainer.GetPrice(_enemyId, _upgradeType, save.upgradesBoughtNum);
        RefreshView(paramName, value, nextValue, bought, max, isMax, price, maxValue);
    }
} 