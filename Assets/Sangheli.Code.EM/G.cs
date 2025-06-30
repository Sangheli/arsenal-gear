using Cysharp.Threading.Tasks;
using SangheliCode;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

[RequireComponent(typeof(SceneContainer))]
public class G : MonoBehaviour
{
    public GameObject debugConsole;

    public RoundManager roundManager;
    public StartGameView startGameView;
    public CursorController playerCursor;
    public UIManager uiManager;
    public EnergySpawner energySpawner;
    public DistortionUI distortionUI;
    public EndRoundView endRoundView;
    public UpgradeView upgradeView;
    public EnemyContainer enemyContainer;
    public AICursorManager aiCursorManager;
    
    public PrestigeManager prestigeManager;

    public SceneContainer Container;
    public DataContainer Data;

    //-----------------------------------------------------------
    
    public static G I;
    public readonly IDataSaveProvider SaveProvider = new DataSavePrefsProvider();
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    public bool ActiveForRuntimeButtons { get; private set; }
    
    public SangheliObjectPool ObjectPool;
    public readonly ShieldManager ShieldManager = new();
    public readonly GameTimeManager GameTimeManager = new();
    public readonly EnergyManager EnergyManager = new ();
    public readonly HPManager HpManager = new();
    
    [Space]
    public UpgradePriceMultiplierManager upgradePriceMultiplierManager = new();

    private void OnValidate()
    {
        Container ??= GetComponent<SceneContainer>();
    }
    
    private void LateUpdate()
    {
        if(I ==null)
            return;
    }

#pragma warning disable CS4014
    private void Awake()
    {
#if !UNITY_EDITOR
        if(debugConsole!=null) Instantiate(debugConsole);
#endif

#if UNITY_EDITOR
        Application.runInBackground = true;
#endif
        
        StartGame();
    }
#pragma warning restore CS4014
    
    private async UniTaskVoid StartGame()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        I = this;
        OnFirstLoad();
    }

    private void OnFirstLoad()
    {
        enemyContainer.UpdateEnemyIndices();
        playerCursor.view.hudView.Toggle(false);
        ObjectPool = new SangheliObjectPool();
        SaveProvider.OnSaveCallback = AnalyticsHelper.Checkpoint;
        endRoundView.Callback = roundManager.ClearAll;
        energySpawner.OnLoadInit();
        playerCursor.Toggle(false);
        uiManager.Toggle(false);
        startGameView.Toggle(true);
        upgradeView.Toggle(false);
        
        SaveProvider.Get();
        
        prestigeManager.OnLoadInit();
    }

    public void StartRunFirstTime()
    {
        var save = SaveProvider.Get();
        save.GetEnemyUpgradeContainer().Init(enemyContainer.List);
        GameTimeManager.Restore(save.timeSinceStart);
        StartRun();
    }

    public void StartRun()
    {
        aiCursorManager.StartRound();
        playerCursor.view.hudView.Toggle(false);
        EnergyManager.StartRound();
        prestigeManager.StartRound();
        HpManager.StartRound();
        uiManager.StartRound();
        ShieldManager.StartRound();
        energySpawner.StartRound();
        
        prestigeManager.ApplyPrestigeColor();
        
        playerCursor.Toggle(true);
        uiManager.Toggle(true);
        startGameView.Toggle(false);
        endRoundView.Toggle(false);
        upgradeView.Toggle(false);

        roundManager.StartRound();
    }

    public void StopRun()
    {
        prestigeManager.StopRound();
        energySpawner.StopRound();
        upgradeView.Toggle(true);
        upgradeView.Init();
        aiCursorManager.ToggleAll(false);
        playerCursor.view.hudView.Toggle(false);
    }
}