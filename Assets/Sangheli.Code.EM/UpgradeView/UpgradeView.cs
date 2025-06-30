using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpgradeView : MonoBehaviour
{
    [SerializeField] private UpgradeStateView _upgradeStatePrefab;
    [SerializeField] private EnemyUpgradeStateView _enemyUpgradeStatePrefab;
    [SerializeField] private Transform _container;
    [SerializeField] private Text moneyText;
    [SerializeField] private Button toggleFilterButton;
    [SerializeField] private Text toggleFilterButtonText;
    
    [SerializeField] private Button cheatButton;

    // Добавляем элементы для сохранения
    [SerializeField] private Button saveButton;
    [SerializeField] private GameObject saveNotification;
    
    // Параметры анимации сохранения
    [SerializeField] private float saveNotificationDuration = 2f;
    [SerializeField] private float bounceDuration = 0.5f;
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] private Text dpmText;
    
    [Space]
    [SerializeField] private Text prestigeText;
    [SerializeField] private Button prestigeMinusButton;
    [SerializeField] private Button prestigePlusButton;
    [SerializeField] private Text priceMultiplierText;
    [SerializeField] private Text prestigePointsText;
    
    public Button startButton;
    
    private List<BaseUpgradeStateView> _spawned;
    private bool _showOnlyActive;
    private const int CheatEnergyAmount = 10000000;
    
    private void Awake()
    {
        toggleFilterButton.onClick.AddListener(ToggleFilter);
        UpdateToggleButtonText();
        cheatButton.gameObject.SetActive(false);
        startButton.onClick.AddListener(OnStart);
        
        if (saveButton != null) saveButton.onClick.AddListener(OnSaveButtonClicked);
        if (saveNotification != null) saveNotification.gameObject.SetActive(false);
        
        if (prestigeText != null)
            prestigeText.text = "";
        if (prestigeMinusButton != null) prestigeMinusButton.gameObject.SetActive(false);
        if (prestigePlusButton != null) prestigePlusButton.gameObject.SetActive(false);
#if UNITY_EDITOR
        cheatButton.gameObject.SetActive(true);
        cheatButton.onClick.AddListener(AddCheatEnergy);
        if (prestigeMinusButton != null) {
            prestigeMinusButton.gameObject.SetActive(true);
            prestigeMinusButton.onClick.AddListener(() => ChangePrestige(-1));
        }
        if (prestigePlusButton != null) {
            prestigePlusButton.gameObject.SetActive(true);
            prestigePlusButton.onClick.AddListener(() => ChangePrestige(1));
        }
#endif
    }
    
    private void ToggleFilter()
    {
        _showOnlyActive = !_showOnlyActive;
        UpdateToggleButtonText();
        RefreshAll();
    }
    
    private void UpdateToggleButtonText()
    {
        if (toggleFilterButtonText != null)
            toggleFilterButtonText.text = _showOnlyActive ? "Show All" : "Show Active Only";
    }
    
    private void ApplyFilter()
    {
        if (_spawned == null) return;
        foreach (var x in _spawned)
        {
            var shouldShow = !_showOnlyActive || !x.IsDisabled;
            x.gameObject.SetActive(shouldShow);
        }
    }
    
#if UNITY_EDITOR
    private void AddCheatEnergy()
    {
        G.I.SaveProvider.Get().totalEnergy += CheatEnergyAmount;
        RefreshAll();
    }

    private void ChangePrestige(int delta)
    {
        G.I.prestigeManager.IncreasePrestige(delta);
        G.I.prestigeManager.ApplyPrestigeColor();
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
        int prestige = G.I.SaveProvider.Get().level;
        foreach (var enemyId in upgradeContainer.GetAllEnemyIds())
        {
            foreach (var upgradeType in upgradeContainer.GetAvailableUpgradeTypes(enemyId, prestige))
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
        saveNotification.gameObject.SetActive(false);
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
            prestigeText.text = $"Level: {G.I.SaveProvider.Get().level+1}";
        
            prestigePointsText.text = $"Prestige Points: {NumberFormatter.Format(G.I.SaveProvider.Get().totalPrestigePoints, 0)}";
        
            float multiplier = G.I.upgradePriceMultiplierManager.GetUpgradesPriceGlobalMultiplier();
            priceMultiplierText.text = $"Price Multiplier: x{multiplier:F2}";
        
        foreach (var x in _spawned) 
            x.Refresh();
        
        ApplyFilter();
    }

    private void OnSaveButtonClicked() => StartCoroutine(SaveGameWithAnimation());

    private IEnumerator SaveGameWithAnimation()
    {
        SaveSystem.Save();
        ShowSaveNotification();
        yield return new WaitForSeconds(saveNotificationDuration);
        if (saveNotification != null)
            saveNotification.gameObject.SetActive(false);
    }
    
    private void ShowSaveNotification()
    {
        if (saveNotification == null) 
            return;
        
        saveNotification.gameObject.SetActive(true);
        saveNotification.transform.localScale = Vector3.zero;
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.checkpoint);
        StartCoroutine(GameAnimation.Bounce(saveNotification.transform, Vector3.one, bounceScale, bounceDuration));
    }
}