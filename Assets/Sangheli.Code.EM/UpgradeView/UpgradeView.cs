using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeView : MonoBehaviour
{
    [SerializeField] private UpgradeStateView _upgradeStatePrefab;
    [SerializeField] private EnemyUpgradeStateView _enemyUpgradeStatePrefab;
    [SerializeField] private Transform _container;
    [SerializeField] private Text moneyText;
    
    [SerializeField] private Button cheatButton;
    
    [SerializeField] private Text dpmText;
    
    [Space]
    [SerializeField] private Text prestigeText;
    [SerializeField] private Button prestigeMinusButton;
    [SerializeField] private Button prestigePlusButton;
    [SerializeField] private Text priceMultiplierText;
    
    public Button startButton;
    
    private List<BaseUpgradeStateView> _spawned;
    private const int CheatEnergyAmount = 10000000;
    
    private void Awake()
    {
        cheatButton.gameObject.SetActive(false);
        startButton.onClick.AddListener(OnStart);
        
        if (prestigeText != null)
            prestigeText.text = "";
        if (prestigeMinusButton != null) prestigeMinusButton.gameObject.SetActive(false);
        if (prestigePlusButton != null) prestigePlusButton.gameObject.SetActive(false);
#if UNITY_EDITOR
        cheatButton.gameObject.SetActive(true);
        cheatButton.onClick.AddListener(AddCheatEnergy);
        if (prestigeMinusButton != null) {
            prestigeMinusButton.gameObject.SetActive(true);
            prestigeMinusButton.onClick.AddListener(() => ChangeLevel(-1));
        }
        if (prestigePlusButton != null) {
            prestigePlusButton.gameObject.SetActive(true);
            prestigePlusButton.onClick.AddListener(() => ChangeLevel(1));
        }
#endif
    }
    
#if UNITY_EDITOR
    private void AddCheatEnergy()
    {
        G.I.SaveProvider.Get().totalEnergy += CheatEnergyAmount;
        RefreshAll();
    }

    private void ChangeLevel(int delta)
    {
        G.I.prestigeManager.ChangeLevel(delta);
        G.I.prestigeManager.ApplyLevelColor();
        SaveSystem.Save();
        RefreshAll();
    }
#endif
    
    private void GenerateUpgradeViews()
    {
        if(_spawned!=null)
            return;
        
        _spawned = new List<BaseUpgradeStateView>();
        GeneratePlayerUpgradeViews();
        GenerateEnemyUpgradeViews();
    }
    
    private void GeneratePlayerUpgradeViews()
    {
        foreach (var key in G.I.SaveProvider.Get().GetParamContainer().GetAllParamKeys())
        {
            var view = Instantiate(_upgradeStatePrefab, _container);
            view.Init(key, RefreshAll);
            _spawned.Add(view);
        }
    }
    
    private void GenerateEnemyUpgradeViews()
    {
        var upgradeContainer = G.I.SaveProvider.Get().GetEnemyUpgradeContainer();
        foreach (var enemyId in upgradeContainer.GetAllEnemyIds())
        {
            foreach (var upgradeType in upgradeContainer.GetAvailableUpgradeTypes(enemyId))
            {
                var view = Instantiate(_enemyUpgradeStatePrefab, _container);
                view.Init(enemyId, upgradeType, RefreshAll);
                _spawned.Add(view);
            }
        }
    }

    public void Init()
    {
        UpgradePitchHelper.ResetAll();
        // Устанавливаем флаг посещения меню апгрейдов
        var save = G.I.SaveProvider.Get();
        if (!save.hasVisitedUpgradeMenu)
        {
            save.hasVisitedUpgradeMenu = true;
            SaveSystem.Save();
        }
        GenerateUpgradeViews();
        RefreshAll();
    }

    private void OnStart()
    {
        G.I.StartRun();
    }

    public void Toggle(bool state) => gameObject.SetActive(state);

    private void RefreshAll()
    {
        if (moneyText != null)
            moneyText.text = $"Energy: {NumberFormatter.Format(G.I.SaveProvider.Get().totalEnergy, 2)}";

        // Обновляем DPS
        if (dpmText != null)
        {
            var (dps, dpsCrit) = DPSManager.GetPlayerDps();
            dpmText.text = $"DPS: {NumberFormatter.Format(dps)}/Crit: {NumberFormatter.Format(dpsCrit)}";
        }

        if (prestigeText != null)
            prestigeText.text = $"Level: {G.I.SaveProvider.Get().level + 1}";

        var upgradesBoughtNum =  G.I.SaveProvider.Get().upgradesBoughtNum;
        var multiplier=  Mathf.Pow(G.I.upgradePriceMultiplierManager.GetUpgradesPriceGlobalMultiplier(), upgradesBoughtNum);
        
        priceMultiplierText.text = $"Price Mult: x{multiplier:F2}";

        foreach (var x in _spawned)
            x.Refresh();
    }
}