using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public AICursorManager aiCursorManager;
    
    public PrestigeManager prestigeManager;
    public LevelStringManager tutorial;
    public BossManager bossManager;

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
    public int maxLevel;
    
    public LevelDataConfigRuntime levelDataConfig { get; set; }

    public bool CanPlay { get; private set; }

    private void OnValidate()
    {
        Container ??= GetComponent<SceneContainer>();
    }
    
    private void LateUpdate()
    {
        if(I ==null)
            return;
        
        HpManager?.UpdateHp();
        EnergyManager.LateUpdate();
        ShieldManager.LateUpdate();
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
        tutorial.OnFirstLoad();
        Data.enemyContainer.UpdateEnemyIndices();
        playerCursor.view.hudView.Toggle(false, 0, false);
        ObjectPool = new SangheliObjectPool();
        SaveProvider.OnSaveCallback = AnalyticsHelper.Checkpoint;
        energySpawner.OnLoadInit();
        playerCursor.Toggle(false);
        uiManager.Toggle(false);
        startGameView.Toggle(true);
        startGameView.saveInfo.FadeOut(2.0f,1);
        upgradeView.Toggle(false);
        endRoundView.distortion.OnLoadInit();
        
        SaveProvider.Get();
        
        levelDataConfig = new LevelDataConfigRuntime(Data.levelDataConfigSO);
        
        prestigeManager.OnLoadInit();
        bossManager = GetComponent<BossManager>();
    }

    public void StartRunFirstTime()
    {
        var save = SaveProvider.Get();
        save.GetEnemyUpgradeContainer().Init(Data.enemyContainer.List);
        GameTimeManager.Init(save.timeSinceStart);
        StartRun();
    }

    public void StartRun()
    {
        SaveSystem.Save();
        StartCoroutine(StartRunSequence());
    }

    private IEnumerator StartRunSequence()
    {
        CanPlay = false;
        distortionUI.DoBlock(true);
        
        yield return distortionUI.DoFade(forward: true);
        
        bossManager.ResetBoss();
        endRoundView.OnStartRound();
        tutorial.OnStartRound();
        playerCursor.view.hudView.damageTextManager.StartRound();
        aiCursorManager.OnStartRound();
        playerCursor.view.hudView.Toggle(false, 0, false);
        EnergyManager.OnStartRound();
        prestigeManager.OnStartRound();
        HpManager.OnStartRound();
        uiManager.OnStartRound();
        ShieldManager.OnStartRound();
        energySpawner.OnStartRound();
        
        prestigeManager.ApplyLevelColor();
        
        playerCursor.Toggle(true);
        uiManager.Toggle(true);
        startGameView.Toggle(false);
        endRoundView.Toggle(false);
        upgradeView.Toggle(false);
        aiCursorManager.ToggleAllHud(true);
        roundManager.OnStartRound();
        
        yield return new WaitForSeconds(distortionUI.endRoundWaitTime);
        CanPlay = true;
        yield return distortionUI.DoFade(forward: false);
        
        distortionUI.DoBlock(false);
    }

    public void StopRun(EndRoundFadeEnum fadeType)
    {
        var save = SaveProvider.Get();
        var demoEnd = save.level >= maxLevel && fadeType == EndRoundFadeEnum.Win;
        StartCoroutine( demoEnd ? ShowEndGame() : StopRunSequence(fadeType));
    }
    
    private IEnumerator ShowEndGame()
    {
        var save = SaveProvider.Get();
        save.level = maxLevel - 1;
        SaveSystem.Save();
        
        roundManager.IsAlive = false;
        roundManager.IsAliveForPlay = false;
        prestigeManager.ToggleActive(false);
        playerCursor.ToggleControl(false, true);
        playerCursor.view.hudView.Toggle(false, 0 , false);
        playerCursor.Toggle(false);
        tutorial.OnEndRound();
        endRoundView.distortion.gameObject.SetActive(true);
        endRoundView.distortion.ResetView();
        yield return endRoundView.distortion.DoFade(EndRoundFadeEnum.EndDemo);
        uiManager.Toggle(false);
    }

    private IEnumerator StopRunSequence(EndRoundFadeEnum fadeType)
    {
        roundManager.IsAlive = false;
        roundManager.IsAliveForPlay = false;
        prestigeManager.ToggleActive(false);
        playerCursor.ToggleControl(false, true);

        var list = energySpawner.EnergyList.Where(x => x.IsActive && !x.CanCollect).ToList();
        while (list.Any(x => x.IsActive))
            yield return null;
        
        var save = SaveProvider.Get();
        
        var energyOnStart = save.totalEnergy;
        var roundEnergy = EnergyManager.RoundEnergyCount;
        var bossEnergy = fadeType == EndRoundFadeEnum.Fail ? 0 : prestigeManager.GetBossReward(SaveProvider.Get().level - 1);
        var toCollect = GetAutoCollectEnergy(save, out var energyAutoCollect);

        save.totalEnergy = energyOnStart + roundEnergy + energyAutoCollect + bossEnergy;
        SaveSystem.Save();
        
        yield return GameAnimation.CollectAllEnergyOnScreen(toCollect, energyAutoCollect);
        yield return new WaitForSeconds(uiManager.energyBarUI.duration*2);
        
        playerCursor.view.hudView.Toggle(false, 0 , false);
        playerCursor.Toggle(false);
        tutorial.OnEndRound();
        
        endRoundView.Show(roundEnergy + energyAutoCollect,energyOnStart, bossEnergy, fadeType);
    }

    private List<EnergyView> GetAutoCollectEnergy(DataSave save, out int energyAutoCollect)
    {
        var energyViews = energySpawner.EnergyList;
        var toCollect = energyViews.Where(x => x.CanCollect).ToList();
        var autoCollectPercent = save.GetParamContainer().GetValue("autoCollectCoinsPercent") / 100f;
        energyAutoCollect = Mathf.Min(toCollect.Count, Mathf.CeilToInt(toCollect.Count * autoCollectPercent));
        return toCollect;
    }

    public void GoToUpgrades()
    {
        StartCoroutine(GoToUpgradesCoroutine());
    }

    private IEnumerator GoToUpgradesCoroutine()
    {
        distortionUI.DoBlock(true);
        
        yield return distortionUI.DoFade(forward: true);
        uiManager.Toggle(false);
        startGameView.Toggle(false);
        EnergyManager.OnGoToUpgradeView();
        endRoundView.distortion.gameObject.SetActive(false);
        roundManager.ClearAll();
        endRoundView.Toggle(false);
        prestigeManager.StopRound();
        energySpawner.StopRound();
        upgradeView.Toggle(true);
        upgradeView.Init();
        aiCursorManager.ToggleAll(false);
        aiCursorManager.ToggleAllHud(false);
        playerCursor.view.hudView.Toggle(false, 0, false);
        
        yield return new WaitForSeconds(distortionUI.endRoundWaitTime);
        yield return distortionUI.DoFade(forward: false);
        
        distortionUI.DoBlock(false);
    }
}