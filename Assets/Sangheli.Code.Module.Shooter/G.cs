using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
    typeof(OrientationChecker)
)]

public class G : MonoBehaviour
{
    public GameObject debugConsole;

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
    
    public readonly IAdvertisement Advertisement = new MockAdvertisement();

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
        LocalizationBridge.Localizations = new HashSet<IUILocalization>();
        StartGame();
    }
#pragma warning restore CS4014
    
    private async UniTaskVoid StartGame()
    {
        await UniTask.Yield();
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

    public void OnFirstLoad()
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

    public void StartRun(bool overrideNew, bool playFader = true)
    {
        AnalyticsHelper.OnStartRound();
        G.I.SaveProvider.Get().paramContainer.ResetTemp();
        G.I.SaveProvider.Get().enemyUpgradeContainer.ResetTemp();
        SaveSystem.SaveSilent();
        StartCoroutine(StartRunSequence(playFader, overrideNew));
    }

    private IEnumerator StartRunSequence(bool playFader, bool overrideNew)
    {
        CanPlay = false;
        distortionUI.DoBlock(true);
        localization.UpdateLocalization();
        
        if(playFader)
            yield return distortionUI.DoFade(forward: true);
        
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
        
        yield return new WaitForSeconds(distortionUI.endRoundWaitTime);
        CanPlay = true;
        
        if(playFader)
            yield return distortionUI.DoFade(forward: false);
        
        distortionUI.DoBlock(false);
    }

    public void StopRun(ILevelTargetEnd levelTargetEnd, EndRoundFadeEnum fadeType)
    {
        var save = SaveProvider.Get();
        var demoEnd = save.level >= maxLevel && fadeType == EndRoundFadeEnum.RoundWin;
        StartCoroutine( demoEnd ? ShowEndGame(levelTargetEnd) : StopRunSequence(fadeType, levelTargetEnd));
    }
    
    private IEnumerator ShowEndGame(ILevelTargetEnd levelTargetEnd)
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
        yield return endRoundView.distortion.DoFade(levelTargetEnd, EndRoundFadeEnum.EndGame);
        uiManager.Toggle(false);

        endRoundView.returnToStartButton.gameObject.SetActive(true);
        endRoundView.returnToStartText.text = G.I.localization.Get("return_to_start");
    }

    private IEnumerator StopRunSequence(EndRoundFadeEnum fadeType, ILevelTargetEnd levelTargetEnd)
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
        
        yield return GameAnimation.CollectAllEnergyOnScreen(toCollect, rewardAutoCollect);
        yield return new WaitForSeconds(uiManager.energyBarUI.duration*2);
        
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
        StartCoroutine(GoToUpgradesCoroutine());
    }

    private IEnumerator GoToUpgradesCoroutine()
    {
        var save = I.SaveProvider.Get();
        save.shopVisits++;
        SaveSystem.SaveSilent();
        
        distortionUI.DoBlock(true);
        bossManager.ResetBoss();
        yield return distortionUI.DoFade(forward: true);
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
        
        yield return new WaitForSeconds(distortionUI.endRoundWaitTime);
        yield return distortionUI.DoFade(forward: false);
        
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
        StartRun(false);
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
        }
        else
        {
            StartRunFirstTime(true);
        }
        
        endRoundView.distortion.OnLoadInit();
    }
}