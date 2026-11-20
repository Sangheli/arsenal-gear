using System.Collections;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using SangheliCode;
using SangheliCore;
using SangheliCore.Adv;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(
    typeof(SceneContainer),
    typeof(OrientationChecker),
    typeof(InputDetector)
)]

public class G : MonoBehaviour
{
    public GameObject debugConsole;

    [FormerlySerializedAs("mobileDetector")] public InputDetector inputDetector;
    public CockpitManager cockpitManager;
    public WeaponManager weaponManager;
    public Camera cameraMain;
    public OrientationChecker orientationChecker;
    public LocalizationBridge localization;
    public RoundManager roundManager;
    public StartGameView startGameView;
    public CursorController playerCursor;
    public UIManager uiManager;
    public EnergySpawner energySpawner;
    public DistortionUI distortionUI;
    public EndRoundView endRoundView;
    public UpgradeView upgradeView;
    public AICursorManager aiCursorManager;
    
    [FormerlySerializedAs("prestigeManager")] public LevelTargetManager levelTargetManager;
    public LevelTutorialTextManager tutorial;
    public BossManager bossManager;

    public SceneContainer Container;
    public DataContainer Data;

    public RectTransformMover cockpitMover;
    
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
    public int delim = 25;
    
    public int RestartsFromEndRound { get; set; }
    private CancellationTokenSource _sequenceCancellation;
    
    public LevelDataConfigRuntime LevelDataConfig { get; set; }
    public ArcMoveAnimator arcMoveAnimator;

    public bool CanPlay { get; private set; }
    public float orthoHor = 5;
    public float orthoVert = 10;
    public int maxYBase = 6;
    public int maxXBase = 10;
    public int MaxY { get; private set; }
    public int MaxX { get; private set; }

    public float minZ = 0f;
    public float maxZ = 10;
    public float posY = 0f;

    public float BoundX = 10;

    public Vector2 boundBossMin;
    public Vector2 boundBossMax;
    
    public readonly AdvManager AdvManager = new();

    public RayFXManager rayFXManager;

    public static readonly List<EndRoundFadeEnum> PlayLevelAgainFader = new()
    {
        EndRoundFadeEnum.RoundTerminated,
        EndRoundFadeEnum.RoundFail,
    };

    public string predefinedLang = "en";
    public bool useOrientationChecker = false;
    public LayoutPlaceHolder layoutPlaceHolder;
    
    [Space]
    public bool startGameWithoutMenu = true;
    
    public LevelConfig currentLevel { get; set; }
    
    private void OnValidate()
    {
        orientationChecker ??= GetComponent<OrientationChecker>();
        Container ??= GetComponent<SceneContainer>();
        localization ??= GetComponent<LocalizationBridge>();
        inputDetector ??= GetComponent<InputDetector>();
    }
    
    private void LateUpdate()
    {
        if(I ==null)
            return;
        
        HpManager?.UpdateHp();
        EnergyManager.LateUpdate();
        ShieldManager.LateUpdate();
    }

    private void Awake()
    {
#if !UNITY_EDITOR
        if(debugConsole!=null) Instantiate(debugConsole);
#endif

#if UNITY_EDITOR
        Application.runInBackground = true;
#endif
        LocalizationBridge.Localizations = new HashSet<IUILocalization>();
        _ = StartGame().LogExceptions();
    }
    
    private async Task StartGame()
    {
        await Task.Yield();
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        I = this;
        OnOrientationChanged(isHorizontal: true);

        if (useOrientationChecker)
        {
            orientationChecker.UpdateOrientation += OnOrientationChanged;
            orientationChecker.UpdateOrientation += layoutPlaceHolder.OnOrientationChanged;
            orientationChecker.Detect();
        }

        localization.ShowLangSelector(GetLang());
    }

    private string GetLang()
    {
        string lang = null;
        
#if UNITY_EDITOR
        lang = predefinedLang;
#endif

        return lang;
    }

    private void OnOrientationChanged(bool isHorizontal)
    {
        MaxY = isHorizontal ? maxYBase : maxXBase;
        MaxX = isHorizontal ? maxXBase : maxYBase;
        Container.camera.orthographicSize = isHorizontal ? orthoHor : orthoVert;
    }

    public async Task OnFirstLoad()
    {
        weaponManager.Init();
        endRoundView.OnFirstLoad();
        uiManager.OnFirstLoad();
        upgradeView.OnFirstLoad();
        startGameView.OnFirstLoad();
        tutorial.OnFirstLoad();
        Data.enemyContainer.UpdateEnemyIndices();
        ObjectPool = new SangheliObjectPool();
        SaveProvider.OnSaveAnalyticsCallback = AnalyticsHelper.Checkpoint;
        energySpawner.OnLoadInit();
        playerCursor.Toggle(false);
        uiManager.Toggle(false);
        upgradeView.Toggle(false);
        
        SaveProvider.Get();
        
        LevelDataConfig = new LevelDataConfigRuntime();
        
        levelTargetManager.OnLoadInit();
        bossManager = GetComponent<BossManager>();
        await Task.Yield();
        if (startGameWithoutMenu) OnStartGameWithoutMenu();
        else OnStartGameWithMenu();
    }

    public void StartRunFirstTime(bool newGame)
    {
        CanPlay = false;
        startGameView.Toggle(false);
        var save = SaveProvider.Get();
        save.GetEnemyUpgradeContainer().Init(Data.enemyContainer.List);
        GameTimeManager.Init(save.timeSinceStart);

        if (newGame)
        {
            AnalyticsHelper.OnFirstGameLaunch();
            if (!startGameWithoutMenu) arcMoveAnimator.Play();
            else OnStartRunInstant();
        }
        else
        {
            AnalyticsHelper.OnContinue();
            save.loadTimes++;
            StartRun(overrideNew: true);
        }
    }

    public void StartRun(bool overrideNew, bool playFader = true, bool playFaderBack = true)
    {
        AnalyticsHelper.OnStartRound();
        G.I.SaveProvider.Get().paramContainer.ResetTemp();
        G.I.SaveProvider.Get().enemyUpgradeContainer.ResetTemp();
        SaveSystem.SaveSilent();
        var cancellationToken = BeginSequence();
        _ = StartRunSequence(playFader, overrideNew, playFaderBack, cancellationToken).LogExceptions();
    }

    private async Task StartRunSequence(bool playFader, bool overrideNew, bool playFaderBack, CancellationToken cancellationToken)
    {
        CanPlay = false;
        distortionUI.DoBlock(true);
        localization.UpdateLocalization();
        
        if(playFader)
            await distortionUI.DoFade(forward: true, cancellationToken);
        
        weaponManager.OnStartRound();
        roundManager.OnStartRound(overrideNew);
        bossManager.ResetBoss();
        endRoundView.OnStartRound();
        tutorial.OnStartRound();
        aiCursorManager.OnStartRound();
        EnergyManager.OnStartRound();
        levelTargetManager.OnStartRound();
        HpManager.OnStartRound();
        uiManager.OnStartRound();
        ShieldManager.OnStartRound();
        energySpawner.OnStartRound();
        
        levelTargetManager.ApplyLevelColor();
        
        playerCursor.Toggle(true);
        uiManager.Toggle(true);
        startGameView.Toggle(false);
        endRoundView.Toggle(false);
        upgradeView.Toggle(false);
        
        await Task.Delay(TimeSpan.FromSeconds(distortionUI.endRoundWaitTime), cancellationToken);
        CanPlay = true;
        
        if(playFaderBack)
            await distortionUI.DoFade(forward: false, cancellationToken);
        
        distortionUI.DoBlock(false);
    }

    public void StopRun(ILevelTargetEnd levelTargetEnd, EndRoundFadeEnum fadeType)
    {
        var save = SaveProvider.Get();
        var demoEnd = save.level >= maxLevel && fadeType == EndRoundFadeEnum.RoundWin;
        var cancellationToken = BeginSequence();
        _ = (demoEnd ? ShowEndGame(levelTargetEnd, cancellationToken) : StopRunSequence(fadeType, levelTargetEnd, cancellationToken)).LogExceptions();
    }
    
    private async Task ShowEndGame(ILevelTargetEnd levelTargetEnd, CancellationToken cancellationToken)
    {
        var save = SaveProvider.Get();
        save.level = maxLevel - 1;
        SaveSystem.Save(sendAnalytics: true, endRoundType: EndRoundFadeEnum.EndGame);
        
        roundManager.IsAlive = false;
        roundManager.IsAliveForPlay = false;
        levelTargetManager.Data.ToggleActive(false);
        playerCursor.ToggleControl(false, true);
        playerCursor.Toggle(false);
        tutorial.OnEndRound();
        endRoundView.distortion.gameObject.SetActive(true);
        endRoundView.distortion.ResetView();
        await endRoundView.distortion.DoFade(levelTargetEnd, EndRoundFadeEnum.EndGame, cancellationToken);
        uiManager.Toggle(false);

        endRoundView.returnToStartButton.gameObject.SetActive(true);
        endRoundView.returnToStartText.text = G.I.localization.Get("return_to_start");
    }

    private async Task StopRunSequence(EndRoundFadeEnum fadeType, ILevelTargetEnd levelTargetEnd, CancellationToken cancellationToken)
    {
        roundManager.IsAlive = false;
        roundManager.IsAliveForPlay = false;
        levelTargetManager.Data.ToggleActive(false);
        playerCursor.ToggleControl(false, true);
        uiManager.terminateButton.gameObject.SetActive(false);

        // Disable all enemy attacks when round ends
        roundManager.DisableAllEnemyAttacking();

        foreach (var x in energySpawner.EnergyList)
        {
            x.IsActive = false;
            x.PlayerFound = false;
        }
        
        var save = SaveProvider.Get();
        
        var gameSum = save.totalEnergy;
        var roundReward = EnergyManager.RoundEnergyCount;

        var bossReward = levelTargetEnd?.GetEnergy() ?? 0;
        var toCollect = GetAutoCollectEnergy(save, out var rewardAutoCollect);

        save.totalEnergy = gameSum + roundReward + rewardAutoCollect + bossReward;
        SaveSystem.Save(sendAnalytics: true, fadeType);
        
        await GameAnimation.CollectAllEnergyOnScreen(toCollect, rewardAutoCollect, cancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(uiManager.energyBarUI.duration * 2), cancellationToken);
        
        playerCursor.Toggle(false);
        tutorial.OnEndRound();
        
        endRoundView.Show(roundReward + rewardAutoCollect,gameSum, bossReward, fadeType, levelTargetEnd);
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
        _ = GoToUpgradesAsync().LogExceptions();
    }

    private async Task GoToUpgradesAsync()
    {
        var cancellationToken = BeginSequence();
        var save = I.SaveProvider.Get();
        save.shopVisits++;
        SaveSystem.SaveSilent();
        
        distortionUI.DoBlock(true);
        bossManager.ResetBoss();
        await distortionUI.DoFade(forward: true, cancellationToken);
        uiManager.Toggle(false);
        startGameView.Toggle(false);
        EnergyManager.OnGoToUpgradeView();
        endRoundView.distortion.gameObject.SetActive(false);
        roundManager.ClearAll();
        endRoundView.Toggle(false);
        levelTargetManager.StopRound();
        energySpawner.StopRound();
        upgradeView.Toggle(true);
        upgradeView.Init();
        aiCursorManager.ToggleAll(false);
        
        await Task.Delay(TimeSpan.FromSeconds(distortionUI.endRoundWaitTime), cancellationToken);
        await distortionUI.DoFade(forward: false, cancellationToken);
        
        distortionUI.DoBlock(false);
        
        AnalyticsHelper.OnShopEntered();
    }

    [Header("Level Progression Curves")]
    public AnimationCurve progressionRewardCurve = AnimationCurve.Linear(0, 1, 1, 2);
    
    public static void ChangeLevel(int delta = 1)
    {
        G.I.RestartsFromEndRound = 0;
        var saveData = G.I.SaveProvider.Get();
        int previousLevel = saveData.level;
        saveData.level+=delta;
        saveData.level = Mathf.Max(saveData.level, 0);
        
        // Track level progression milestones
        if (delta > 0)
        {
            AnalyticsHelper.CheckLevelProgressMilestones(previousLevel, saveData.level);
        }
    }
    
    private void OnStartRunInstant()
    {
        SaveProvider.CreateNew();
        SaveProvider.Get().saveVersion = GameVersionConstants.CURRENT_GAME_VERSION;
        SaveSystem.SaveSilent();
        StartRun(false, playFader: false, playFaderBack: true);
    }
    
    private void OnStartGameWithMenu()
    {
        startGameView.Toggle(true);
        startGameView.DoSaveInfoFadeOut(2.0f,1);
        endRoundView.distortion.OnLoadInit();
    }

    private void OnStartGameWithoutMenu()
    {
        var datetime = PlayerPrefs.GetString(SaveSystem.GetKey(), null);
        bool hasSave = !string.IsNullOrEmpty(datetime);

        if (hasSave)
        {
            startGameView.Toggle(true);
            startGameView.DoSaveInfoFadeOut(2.0f, 1);
            _ = G.I.distortionUI.StartFadeOut().LogExceptions();
        }
        else
        {
            StartRunFirstTime(true);
        }
        
        endRoundView.distortion.OnLoadInit();
    }

    public void OnGameContinue()
    {
        AnalyticsHelper.OnContinueRewarded();
        G.I.HpManager.OnRespawn();
        
        // Hide end round view
        G.I.endRoundView.Toggle(false);
        
        // Resume gameplay
        G.I.roundManager.IsAlive = true;
        G.I.roundManager.IsAliveForPlay = true;
        G.I.playerCursor.ToggleControl(true, true);
        G.I.playerCursor.Toggle(true);
        G.I.uiManager.Toggle(true);
        
        // Reset distortion
        G.I.endRoundView.distortion.ResetView();
        G.I.endRoundView.distortion.gameObject.SetActive(false);
        
        // Reset all existing coins to make them collectible again
        foreach (var energy in G.I.energySpawner.EnergyList.Where(energy => energy != null))
        {
            energy.Init();
            energy.coinFloorHandler.Toggle(true);
        }
        
        G.I.roundManager.StartEnemyAttackDelay(G.I.Data.continueSettings.enemyAttackDelay);
    }

    private CancellationToken BeginSequence()
    {
        _sequenceCancellation?.Cancel();
        _sequenceCancellation?.Dispose();
        _sequenceCancellation = new CancellationTokenSource();
        return _sequenceCancellation.Token;
    }

    private void OnDestroy()
    {
        _sequenceCancellation?.Cancel();
        _sequenceCancellation?.Dispose();
    }
}