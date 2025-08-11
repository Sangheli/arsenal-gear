using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeView : MonoBehaviour
{
    [SerializeField] private Text moneyText;
    [SerializeField] private Text dpmText;
    [SerializeField] private Text critText;
    [SerializeField] private Text hpText;
    [SerializeField] private Text prestigeText;
    [SerializeField] private Text startRoundText;
    
    [Space]
    [SerializeField] private UpgradeRowView _upgradeRowPrefab;
    [SerializeField] private Transform _container;
    
    [SerializeField] private Button cheatButton;
    
    [Space]
    [SerializeField] private Button prestigeMinusButton;
    [SerializeField] private Button prestigePlusButton;
    
    public Button startButton;
    
    private List<BaseUpgradeStateView> _spawned;
    private Dictionary<int, UpgradeRowView> _upgradeRows;
    private const int CheatEnergyAmount = 10000000;
    
    private void Awake()
    {
        cheatButton.gameObject.SetActive(false);
        startButton.onClick.AddListener(OnStart);
        
        if (prestigeText != null)
            prestigeText.text = "";
        if (prestigeMinusButton != null) prestigeMinusButton.gameObject.SetActive(false);
        if (prestigePlusButton != null) prestigePlusButton.gameObject.SetActive(false);
        InitCheatButtons();
    }

#if UNITY_EDITOR
    private void InitCheatButtons()
    {
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
    }
    
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
#else
    private void InitCheatButtons() { }
#endif
    
    private void GenerateUpgradeViews()
    {
        if(_spawned!=null)
            return;
        
        _spawned = new List<BaseUpgradeStateView>();
        _upgradeRows = new Dictionary<int, UpgradeRowView>();
        GeneratePlayerUpgradeViews();
        GenerateEnemyUpgradeViews();
    }
    
    private void GeneratePlayerUpgradeViews()
    {
        var paramContainer = G.I.SaveProvider.Get().GetParamContainer();
        foreach (var key in paramContainer.GetAllParamKeys())
        {
            var paramInfo = G.I.Data.ParamData.GetParamInfo(key);
            if (paramInfo != null)
            {
                var row = GetOrCreateUpgradeRow(paramInfo.row);
                row.SpawnUpgradeView(key, RefreshAll);
                
                // Add the spawned view to our tracking list
                if (row.views.Count > 0)
                {
                    _spawned.Add(row.views[row.views.Count - 1]);
                }
            }
        }
    }
    
    private void GenerateEnemyUpgradeViews()
    {
        var upgradeContainer = G.I.SaveProvider.Get().GetEnemyUpgradeContainer();
        
        // Find the highest row index used by player upgrades
        int lastPlayerUpgradeRow = 0;
        var paramContainer = G.I.SaveProvider.Get().GetParamContainer();
        foreach (var key in paramContainer.GetAllParamKeys())
        {
            var paramInfo = G.I.Data.ParamData.GetParamInfo(key);
            if (paramInfo != null && paramInfo.row > lastPlayerUpgradeRow)
            {
                lastPlayerUpgradeRow = paramInfo.row;
            }
        }
        
        // Assign row numbers for enemy upgrades starting after the last player upgrade row
        foreach (var enemyId in upgradeContainer.GetAllEnemyIds())
        {
            var enemyData = G.I.Data.enemyContainer.List.Find(e => e.EnemyId == G.I.Data.enemyContainer.List[enemyId].EnemyId);
            if (enemyData == null) continue;
            
            foreach (var upgradeType in upgradeContainer.GetAvailableUpgradeTypes(enemyId))
            {
                var upgradeInfo = enemyData.UpgradeInfos.Find(x => x.Type == upgradeType);
                if (upgradeInfo != null)
                {
                    // Use automatic row numbering for enemy upgrades
                    // Start after the last player upgrade row
                    var row = GetOrCreateUpgradeRow(lastPlayerUpgradeRow + 1 + enemyId);
                    row.SpawnEnemyUpgradeView(enemyId, upgradeType, RefreshAll);
                    
                    // Add the spawned view to our tracking list
                    if (row.views.Count > 0)
                    {
                        _spawned.Add(row.views[row.views.Count - 1]);
                    }
                }
            }
        }
    }
    
    private UpgradeRowView GetOrCreateUpgradeRow(int rowIndex)
    {
        if (_upgradeRows.TryGetValue(rowIndex, out var existingRow))
        {
            return existingRow;
        }
        
        var newRow = Instantiate(_upgradeRowPrefab, _container);
        newRow.Initialize(rowIndex);
        _upgradeRows[rowIndex] = newRow;
        return newRow;
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
        G.I.Advertisement.ShowInterstitial();
        G.I.StartRun();
    }

    public void Toggle(bool state)
    {
        gameObject.SetActive(state);
        if (state)
        {
            startRoundText.text = G.I.localization.Get("start_round");
        }
    }

    private void RefreshAll()
    {
        if (moneyText != null)
            moneyText.text = $"{G.I.localization.Get("energy")}: {NumberFormatter.Format(G.I.SaveProvider.Get().totalEnergy, 2)}";

        // Обновляем DPS
        if (dpmText != null)
        {
            var (dps, dpsCrit) = DPSManager.GetPlayerDps();
            dpmText.text =  $"{G.I.localization.Get("dps")}: {NumberFormatter.Format(dps)}";
            critText.text = $"{G.I.localization.Get("crit")}: {NumberFormatter.Format(dpsCrit)}";
        }

        // Обновляем HP
        if (hpText != null)
        {
            var save = G.I.SaveProvider.Get();
            var totalHp = HPManager.Keys.Sum(key => save.GetParamContainer().GetValue(key));
            hpText.text = $"{G.I.localization.Get("hp")}: {NumberFormatter.Format(totalHp, 1)}";
        }

        if (prestigeText != null)
            prestigeText.text = $"{G.I.localization.Get("level")}: {G.I.SaveProvider.Get().level + 1}";

        foreach (var x in _spawned)
            x.Refresh();
    }
}