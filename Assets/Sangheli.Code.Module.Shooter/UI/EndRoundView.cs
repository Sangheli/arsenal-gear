using System.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class EndRoundView : MonoBehaviour
{
    //text
    public Text totalRoundText;
    public Text totalGameText;
    public Text rewardText;
    public Text rewardBossText;
    
    [Space]
    public Text totalTextCoin;
    public Text totalGameTextCoin;
    public Text rewardTextCoin;
    public Text rewardBossTextCoin;

    [Space]
    public Text buttonStartNewRoundText;
    public Text upgradesText;
    public Text checkPointText;
    
    [Space]
    public GameObject View;
    public Button buttonEndRound;
    public Button buttonStartNewRound;
    public float animationTime = 1;
    public float intervalNum = 10;
    
    [Space]
    // Continue feature
    public Button continueButton;
    public Text continueButtonText;
    
    private ContinueManagerBase _continueManagerBase = new();
    
    public EndRoundViewDistortion distortion;
    
    [Space]
    // Параметры баунс анимации
    public float waitBeforeAnim = 0.5f;

    [Space]
    public Button returnToStartButton;
    public Text returnToStartText;
    
    [Space]
    public Button rateMeButton;
    public Text rateMeText;
    public RateMeSO rateMeData;
    
    private Color _buttonStartNewRoundOriginalColor;
    private Color _buttonStartNewRoundOriginalColor2;
    private Color _buttonStartNewRoundTextOriginalColor;
    
    private const string PrefixRound = "round_reward";
    private const string PrefixBoss = "boss_reward";
    private const string PrefixTotal = "total";
    private const string PrefixTotalSumGame = "totalSumGame";
    private const string PrefixContinue = "continue_for_reward";
    
    private CancellationTokenSource _showCancellation;

    public UIFaderOut saveInfo;

    public Image startChild;
    
    private void OnValidate()
    {
        buttonStartNewRoundText ??= buttonStartNewRound.GetComponentInChildren<Text>();
    }

    public void OnFirstLoad()
    {
        _buttonStartNewRoundOriginalColor = buttonStartNewRound.image.color;
        _buttonStartNewRoundOriginalColor2 = startChild.color;
        _buttonStartNewRoundTextOriginalColor = buttonStartNewRoundText.color;
        buttonEndRound.onClick.AddListener(OnExitFromRound);
        buttonStartNewRound.onClick.AddListener(OnStartNewRound);
        
        // Initialize continue button
        continueButton.onClick.AddListener(OnContinue);
        
        if (returnToStartButton != null)
            returnToStartButton.onClick.AddListener(OnReturnToStartScreen);
        
        if (rateMeButton != null)
            rateMeButton.onClick.AddListener(OnRateMeClicked);
        
        Toggle(false);
        
        buttonStartNewRound.image.color = Color.clear;
        buttonStartNewRoundText.color = Color.clear;
    }

    public void OnStartRound()
    {
        distortion.gameObject.SetActive(false);
        _continueManagerBase.ContinueManager.Reset();
    }

    private void OnExitFromRound()
    {
        G.I.AdvManager.Advertisement.ShowInterstitial();
        StopShowAnimation();
        G.I.GoToUpgrades();
    }

    private void OnStartNewRound()
    {
        G.I.RestartsFromEndRound++;
        
        G.I.AdvManager.Advertisement.ShowInterstitial();
        StopShowAnimation();
        G.I.StartRun(overrideNew: false);
    }

    private void OnReturnToStartScreen()
    {
        // Clear all enemies and energy
        G.I.roundManager.ClearAll();
        foreach (var energy in G.I.energySpawner.EnergyList)
        {
            if (energy != null && energy.gameObject.activeSelf)
            {
                energy.IsActive = false;
                energy.PlayerFound = false;
                G.I.ObjectPool.Return(energy);
            }
        }
        
        // Disable game UI
        G.I.uiManager.Toggle(false);
        
        // Show start screen
        G.I.startGameView.Toggle(true);
        
        // Hide end round view
        Toggle(false);
        
        distortion.ResetView();
        distortion.gameObject.SetActive(false);
    }

    public void Toggle(bool state, int level = -1)
    {
        if (state)
        {
            totalRoundText.text = $"{G.I.localization.Get(PrefixTotal)}";
            totalGameText.text = $"{G.I.localization.Get(PrefixTotalSumGame)}";
            rewardText.text = $"{G.I.localization.Get(PrefixRound)}";
            rewardBossText.text = $"{G.I.localization.Get(PrefixBoss)}";
            
            upgradesText.text = G.I.localization.Get("go_to_upgrades");
            checkPointText.text = G.I.localization.Get("checkpoint_done");
            
            // Set continue button text if the button exists
                continueButtonText.text = G.I.localization.Get(PrefixContinue);
            
            // Set rate me button text if the button exists
            if (rateMeText != null)
                rateMeText.text = G.I.localization.Get("rate_me");
        }
        
        View.SetActive(state);
        saveInfo.gameObject.SetActive(state);
        returnToStartButton.gameObject.SetActive(false);
    }

    public void Show(int rewardCollected, int energyOnStart, int bossReward, EndRoundFadeEnum fadeType, ILevelTargetEnd levelTargetEnd)
    {
        StopShowAnimation();
        _showCancellation = new CancellationTokenSource();
        _ = ShowAsync(rewardCollected, energyOnStart, bossReward, fadeType, levelTargetEnd, _showCancellation.Token);
    }

    private async Task ShowAsync(int rewardCollected, int energyOnStart,int bossReward, EndRoundFadeEnum fadeType, ILevelTargetEnd levelTargetEnd, CancellationToken cancellationToken)
    {
        totalGameTextCoin.text = $"{NumberFormatter.Format(energyOnStart, 1)}";
        
        buttonStartNewRound.image.color = Color.clear;
        buttonStartNewRoundText.color = Color.clear;
        startChild.color = Color.clear;
        
        rewardTextCoin.text = $"{NumberFormatter.Format(rewardCollected, 1)}";
        rewardBossTextCoin.text = $"{NumberFormatter.Format(bossReward, 1)}";

        var save = G.I.SaveProvider.Get();
        var showGoToUpgradesButton = true;
        var showNextRound = true;

        if (save != null)
        {
            var levelId = fadeType == EndRoundFadeEnum.RoundWin ? save.level - 1 : save.level;
            var levelConfig = G.I.LevelDataConfig.Get(levelId, false);
            if (levelConfig != null)
            {
                showGoToUpgradesButton = levelConfig.showGoToUpgradesButton;
                if (showGoToUpgradesButton && !save.hasVisitedUpgradeMenu) showNextRound = false;
            }
        }

        var subLevel =fadeType == EndRoundFadeEnum.RoundWin ? save.level : save.level+1;
        var every5 = subLevel % 5 == 0;
        var afterTutor = subLevel >= 10;
        var canContinue = _continueManagerBase.ContinueManager.CanContinue(save.level);
        continueButton.gameObject.SetActive( every5 &&  afterTutor && fadeType == EndRoundFadeEnum.RoundFail && canContinue);
        rateMeButton.gameObject.SetActive(!G.I.Data.storeConfig.DisableForStore && subLevel>=20 && every5);

        if (showNextRound)
        {
            buttonStartNewRound.image.color = _buttonStartNewRoundOriginalColor;
            startChild.color = _buttonStartNewRoundOriginalColor2;
            buttonStartNewRoundText.color = _buttonStartNewRoundTextOriginalColor;
        }

        buttonEndRound.gameObject.SetActive(showGoToUpgradesButton);

        distortion.gameObject.SetActive(true);
        distortion.ResetView();
        await distortion.DoFade(levelTargetEnd, fadeType, cancellationToken);
        Toggle(true, save.level);

        buttonStartNewRoundText.text = G.PlayLevelAgainFader.Contains(fadeType) ? G.I.localization.Get("play_level_again") : $"{G.I.localization.Get("play_next_level")} {save.level + 1}";

        saveInfo.Show();
        saveInfo.FadeOut(2,1);

        await ShowRewardAnim(rewardCollected, 0, energyOnStart, rewardTextCoin, cancellationToken);
        await ShowRewardAnim(bossReward, rewardCollected, energyOnStart+rewardCollected, rewardBossTextCoin, cancellationToken);
    }
    
    private async Task ShowRewardAnim(int reward, float startRewardForRound, float startRewardForGame, Text textToDecrease, CancellationToken cancellationToken)
    {
        if (reward > 0)
        {
            await ShowCoinAnim(reward, startRewardForRound, startRewardForGame, textToDecrease, cancellationToken);
        }
        else if (textToDecrease == rewardTextCoin) // Только для обычной награды обновляем energyText
        {
            totalTextCoin.text = $"{NumberFormatter.Format(reward, 1)}";
            totalGameTextCoin.text = $"{NumberFormatter.Format(startRewardForGame, 1)}";
        }
    }

    private async Task ShowCoinAnim(int energy, float totalEnergy,float gameEnergy, Text textToDecrease, CancellationToken cancellationToken)
    {
        totalTextCoin.text = $"{NumberFormatter.Format(totalEnergy, 1)}";
        totalGameTextCoin.text = $"{NumberFormatter.Format(totalEnergy+gameEnergy, 1)}";

        await Task.Delay(TimeSpan.FromSeconds(waitBeforeAnim), cancellationToken);
        
        float addTemp = energy;
        float stepTime = animationTime / intervalNum;
        float energyPerStep = energy / intervalNum;
        float currentTotal = 0;
        float currentTotalGame = gameEnergy;
        
        for (int i = 0; i < intervalNum; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(stepTime), cancellationToken);
            currentTotal += energyPerStep;
            currentTotalGame += energyPerStep;
            addTemp -=  energyPerStep;
            PrintCoinAnim(textToDecrease, addTemp, currentTotal, currentTotalGame);
            G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.ShowCoinAnim);
        }
        
        PrintCoinAnim(textToDecrease, 0, energy+totalEnergy, energy + totalEnergy + gameEnergy);
    }
    
    private void PrintCoinAnim(Text textToDecrease, float addTemp, float currentTotal, float currentTotalGame)
    {
        textToDecrease.text = $"{NumberFormatter.Format(Mathf.Max(addTemp, 0),1)}";
        totalTextCoin.text = $"{NumberFormatter.Format(currentTotal, 1)}";
        totalGameTextCoin.text = $"{NumberFormatter.Format(currentTotalGame, 1)}";
    }

    private void StopShowAnimation()
    {
        if (_showCancellation == null)
            return;
        
        _showCancellation.Cancel();
        _showCancellation.Dispose();
        _showCancellation = null;
    }

    private void OnDestroy()
    {
        StopShowAnimation();
    }

    private void OnContinue() => _continueManagerBase.ContinueManager.OnContinue();

    private void OnRateMeClicked()
    {
        if (rateMeData != null) MediaUrlOpener.OpenURL(rateMeData.rateURL);
    }
}