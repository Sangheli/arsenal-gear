using UnityEngine;
using UnityEngine.UI;

public class StartGameView : MonoBehaviour
{
    public Button startButton;
    public Text startButtonText;
    public Button loadCheckpointButton;
    public Text loadCheckpointText;
    public GameObject saveStatGO;
    public Text saveStatsText;
    [SerializeField] private Button buttonYoutube;
    [SerializeField] private Button buttonDS;
    public Button buttonGoToUpgrades;
    
    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        loadCheckpointButton.onClick.AddListener(LoadCheckpoint);
        buttonYoutube.onClick.AddListener(MediaUrlOpener.OpenYoutube);
        buttonDS.onClick.AddListener(MediaUrlOpener.OpenDs);
        buttonGoToUpgrades.onClick.AddListener(OnExitFromRound);
    }
    
    private void OnExitFromRound() => G.I.GoToUpgrades();

    private void OnStartButtonClick()
    {
        G.I.SaveProvider.CreateNew();
        SaveSystem.Save();
        G.I.StartRunFirstTime();
    }

    private void LoadCheckpoint()
    {
        var datetime = PlayerPrefs.GetString(SaveSystem.GetKey(), null);
        
        if (string.IsNullOrEmpty(datetime))
        {
            Debug.Log("No checkpoint found to load");
            return;
        }

        G.I.StartRunFirstTime();
    }

    private void UpdateLoadButtonVisibility()
    {
        if (loadCheckpointButton == null) 
            return;
        
        var datetime = PlayerPrefs.GetString(SaveSystem.GetKey(), null);
        bool hasSave = !string.IsNullOrEmpty(datetime);
        
        loadCheckpointButton.gameObject.SetActive(hasSave);
        buttonGoToUpgrades.gameObject.SetActive(hasSave);
        saveStatGO.gameObject.SetActive(hasSave);
        
        // Обновляем текст кнопки старта в зависимости от наличия чекпоинта
        startButtonText.text = hasSave ? "Start New" : "Start";

        if (hasSave)
        {
            G.I.SaveProvider.ApplyFromSave(SaveSystem.GetSlot());
            loadCheckpointText.text = $"Load checkpoint";
            UpdateSaveStats();
        }
    }

    private void UpdateSaveStats()
    {
        // Загружаем данные сейва для отображения статистики
        var saveData = G.I.SaveProvider.Get();
        var paramContainer = saveData.GetParamContainer();
        
        var stats = new System.Text.StringBuilder();
        stats.AppendLine($"Checkpoint: {PlayerPrefs.GetString(SaveSystem.GetKey())}");
        stats.AppendLine($"Level: {saveData.level+1}");
        stats.AppendLine($"Total Energy: {NumberFormatter.Format(saveData.totalEnergy, 0)}");
        stats.AppendLine($"Play time: {GameTimeDisplay.GetFormattedTime(saveData.timeSinceStart)}");

        // Добавляем DPS
        var (dps, dpsCrit) = DPSManager.GetPlayerDps();
        stats.AppendLine($"DPS: {NumberFormatter.Format(dps)}/Crit: {NumberFormatter.Format(dpsCrit)}");

        // Добавляем очки престижа и глобальный множитель цены
        stats.AppendLine($"P Points: {NumberFormatter.Format(saveData.totalPrestigePoints, 0)}");
        float priceMultiplier = G.I.upgradePriceMultiplierManager.GetUpgradesPriceGlobalMultiplier();
        stats.AppendLine($"Price Multiplier: x{priceMultiplier:F2}");

        stats.AppendLine("");
        stats.AppendLine("Player Stats:");
        
        // Добавляем статистику параметров игрока
        foreach (var paramKey in paramContainer.GetAvailableParamKeys())
        {
            var displayName = paramContainer.GetDisplayName(paramKey);
            var currentValue = paramContainer.GetBaseValue(paramKey);
            var boughtLevel = paramContainer.GetBoughtLevel(paramKey);
            var maxLevel = paramContainer.GetMaxLevel(paramKey);
            
            stats.AppendLine($" {displayName}: {NumberFormatter.Format(currentValue)} ({boughtLevel}/{maxLevel})");
        }
        
        stats.AppendLine("");
        stats.AppendLine("Enemy Stats:");
        
        // Добавляем статистику врагов только если достигнут нужный престиж
        var enemyUpgradeContainer = saveData.GetEnemyUpgradeContainer();
        foreach (var enemyId in enemyUpgradeContainer.GetAllEnemyIds())
        {
            // Получаем EnemyData по enemyId
            var enemyDisplayName = enemyUpgradeContainer.GetEnemyDisplayName(enemyId);
            stats.AppendLine($" {enemyDisplayName}:");
            foreach (var upgradeType in enemyUpgradeContainer.GetAvailableUpgradeTypes(enemyId))
            {
                var displayName = enemyUpgradeContainer.GetDisplayName(enemyId, upgradeType);
                var currentValue = enemyUpgradeContainer.GetValue(enemyId, upgradeType);
                var boughtLevel = enemyUpgradeContainer.GetBoughtLevel(enemyId, upgradeType);
                var maxLevel = enemyUpgradeContainer.GetMaxLevel(enemyId, upgradeType);
                
                stats.AppendLine($"  {displayName}: {NumberFormatter.Format(currentValue)} ({boughtLevel}/{maxLevel})");
            }
        }
        
        saveStatsText.text = stats.ToString();
    }

    public void Toggle(bool b)
    {
        gameObject.SetActive(b);
        if (b) UpdateLoadButtonVisibility();
    }
}