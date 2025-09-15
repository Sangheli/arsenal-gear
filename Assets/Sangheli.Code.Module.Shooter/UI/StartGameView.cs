using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;

public class StartGameView : MonoBehaviour
{
    public Text autoSaveInfoText;
    public Button startButton;
    public Text startButtonText;
    public Text upgradeButtonText;
    public Button loadCheckpointButton;
    public Text loadCheckpointText;
    public GameObject saveStatGO;
    public Text saveStatsText;
    [SerializeField] private Button buttonYoutube;
    [SerializeField] private Button buttonDS;
    public Button buttonGoToUpgrades;
    public UIFaderOut saveInfo;
    public bool useMediaButtons = false;
    public bool useAutoSaveInfo = false;
    
    // New levels notification UI element
    public GameObject newLevelsNotification;
    public Text newLevelsNotificationText;

    public UIEffect startButtonUiEffect;
    
    public void OnFirstLoad()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        loadCheckpointButton.onClick.AddListener(LoadCheckpoint);
        buttonGoToUpgrades.onClick.AddListener(OnUpgradesFromStartMenu);

        if (useMediaButtons)
        {
            buttonYoutube.onClick.AddListener(MediaUrlOpener.OpenYoutube);
            buttonDS.onClick.AddListener(MediaUrlOpener.OpenDs);
        }
    }

    private void OnUpgradesFromStartMenu()
    {
        var save = G.I.SaveProvider.Get();
        G.I.GameTimeManager.Init(save.timeSinceStart);
        save.loadTimes++;
        G.I.GoToUpgrades();
    }

    private void OnStartButtonClick()
    {
        G.I.SaveProvider.CreateNew();
        G.I.SaveProvider.Get().saveVersion = GameVersionConstants.CURRENT_GAME_VERSION; // Set the current level version for new games
        SaveSystem.SaveSilent();
        G.I.StartRunFirstTime(true);
    }

    private void LoadCheckpoint()
    {
        var datetime = PlayerPrefs.GetString(SaveSystem.GetKey(), null);
        
        if (string.IsNullOrEmpty(datetime))
        {
            Debug.Log("No checkpoint found to load");
            return;
        }

        G.I.StartRunFirstTime(false);
    }

    private void UpdateLoadButtonVisibility()
    {
        var datetime = PlayerPrefs.GetString(SaveSystem.GetKey(), null);
        bool hasSave = !string.IsNullOrEmpty(datetime);
        
        loadCheckpointButton.gameObject.SetActive(hasSave);
        buttonGoToUpgrades.gameObject.SetActive(hasSave);
        saveStatGO.gameObject.SetActive(hasSave);
        
        // Обновляем текст кнопки старта в зависимости от наличия чекпоинта
        startButtonText.text = hasSave 
            ? G.I.localization.Get("start_new") 
            : G.I.localization.Get("start_game");

        startButtonUiEffect.enabled = !hasSave;
        
        upgradeButtonText.text = G.I.localization.Get("go_to_upgrades");
        
        if(useAutoSaveInfo)
            autoSaveInfoText.text = G.I.localization.Get("auto_save_info_text");

        if (hasSave)
        {
            G.I.SaveProvider.ApplyFromSave(SaveSystem.GetSlot());
            loadCheckpointText.text = G.I.localization.Get("load_checkpoint");
            UpdateSaveStats();
        }
        
        ToggleNewLevelNotifications(hasSave);
        
        var saveData = G.I.SaveProvider.Get();
        saveData.saveVersion = GameVersionConstants.CURRENT_GAME_VERSION;
    }

    private void UpdateSaveStats()
    {
        // Загружаем данные сейва для отображения статистики
        var saveData = G.I.SaveProvider.Get();
        var paramContainer = saveData.GetParamContainer();
        
        var stats = new System.Text.StringBuilder();
        stats.AppendLine($"{G.I.localization.Get("checkpoint")}: {PlayerPrefs.GetString(SaveSystem.GetKey())}");
        stats.AppendLine($"{G.I.localization.Get("level")}: {saveData.level+1}");
        stats.AppendLine($"{G.I.localization.Get("total_energy")}: {NumberFormatter.Format(saveData.totalEnergy, 1)}");
        stats.AppendLine($"{G.I.localization.Get("play_time")}: {GameTimeDisplay.GetFormattedTime(saveData.timeSinceStart)}");

        // Добавляем DPS
        var (dps, dpsCrit) = DPSManager.GetCurrentPlayerDps();
        // stats.AppendLine($"DPS: {NumberFormatter.Format(dps)}/Crit: {NumberFormatter.Format(dpsCrit)}");

        // Добавляем очки престижа и глобальный множитель цены
        var upgradesBoughtNum =  G.I.SaveProvider.Get().upgradesBoughtNum;
        var priceMultiplier=  G.I.upgradePriceMultiplierManager.CalculatePriceMultiplier(upgradesBoughtNum);
        
        // stats.AppendLine($"Price Mult: x{priceMultiplier:F2}");
        //
        // stats.AppendLine("");
        // stats.AppendLine("Player Stats:");
        //
        // // Добавляем статистику параметров игрока
        // foreach (var paramKey in paramContainer.GetAvailableParamKeys())
        // {
        //     var displayName = paramContainer.GetDisplayName(paramKey);
        //     var currentValue = paramContainer.GetValue(paramKey);
        //     
        //     stats.AppendLine($" {displayName}: {NumberFormatter.Format(currentValue)}");
        // }
        //
        // stats.AppendLine("");
        // stats.AppendLine("Enemy Stats:");
        //
        // // Добавляем статистику врагов только если достигнут нужный престиж
        // var enemyUpgradeContainer = saveData.GetEnemyUpgradeContainer();
        // foreach (var enemyId in enemyUpgradeContainer.GetAllEnemyIds())
        // {
        //     // Получаем EnemyData по enemyId
        //     var enemyDisplayName = enemyUpgradeContainer.GetEnemyDisplayName(enemyId);
        //     stats.AppendLine($" {enemyDisplayName}:");
        //     foreach (var upgradeType in enemyUpgradeContainer.GetAvailableUpgradeTypes(enemyId))
        //     {
        //         var displayName = enemyUpgradeContainer.GetDisplayName(enemyId, upgradeType);
        //         var currentValue = enemyUpgradeContainer.GetValue(enemyId, upgradeType);
        //         
        //         stats.AppendLine($"  {displayName}: {NumberFormatter.Format(currentValue)}");
        //     }
        // }
        
        saveStatsText.text = stats.ToString();
    }

    public void Toggle(bool b)
    {
        gameObject.SetActive(b);
        if (b) UpdateLoadButtonVisibility();
    }

    public void DoSaveInfoFadeOut(float duration, float delay)
    {
        if(useAutoSaveInfo)
            saveInfo.FadeOut(duration, delay);
    }

    private void ToggleNewLevelNotifications(bool hasSave)
    {
        var showNew = CanShowNewLevelsNotify(hasSave);
        
        newLevelsNotification.SetActive(showNew);
        
        if (!showNew) return;
        newLevelsNotificationText.text = G.I.localization.Get("new_levels_available");
    }
    
    private static bool CanShowNewLevelsNotify(bool hasSave)
    {
        if (!hasSave) return false;
        var saveData = G.I.SaveProvider.Get();
        return saveData.saveVersion < GameVersionConstants.VERSION_NEW_MAPS_1;
    }
}