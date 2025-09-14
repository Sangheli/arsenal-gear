using System.Collections;
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
    
    private Coroutine _showCoroutine;

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
        if (returnToStartButton != null)
            returnToStartButton.onClick.AddListener(OnReturnToStartScreen);
        
        if (rateMeButton != null)
            rateMeButton.onClick.AddListener(OnRateMeClicked);
        
        Toggle(false);
        
        buttonStartNewRound.image.color = Color.clear;
        buttonStartNewRoundText.color = Color.clear;
    }

    public void OnStartRound() => distortion.gameObject.SetActive(false);

    private void OnExitFromRound()
    {
        G.I.Advertisement.ShowInterstitial();
        StopShowCoroutine();
        G.I.GoToUpgrades();
    }

    private void OnStartNewRound()
    {
        G.I.RestartsFromEndRound++;
        
        G.I.Advertisement.ShowInterstitial();
        StopShowCoroutine();
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
            
            // Set rate me button text if the button exists
            if (rateMeText != null)
                rateMeText.text = G.I.localization.Get("rate_me");
            
            rateMeButton.gameObject.SetActive(!G.I.Data.storeConfig.DisableForStore && level>=20 && level%5==0);
        }
        
        buttonEndRound.interactable = false;
        buttonStartNewRound.interactable = false;
        buttonStartNewRound.interactable = false;
        View.SetActive(state);
        returnToStartButton.gameObject.SetActive(false);
    }

    public void Show(int rewardCollected, int energyOnStart, int bossReward, EndRoundFadeEnum fadeType, ILevelTargetEnd levelTargetEnd)
    {
        StopShowCoroutine();
        _showCoroutine = StartCoroutine(ShowCoroutine(rewardCollected, energyOnStart, bossReward, fadeType, levelTargetEnd));
    }

    private IEnumerator ShowCoroutine(int rewardCollected, int energyOnStart,int bossReward, EndRoundFadeEnum fadeType, ILevelTargetEnd levelTargetEnd)
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

        if (showNextRound)
        {
            buttonStartNewRound.image.color = _buttonStartNewRoundOriginalColor;
            startChild.color = _buttonStartNewRoundOriginalColor2;
            buttonStartNewRoundText.color = _buttonStartNewRoundTextOriginalColor;
        }

        buttonEndRound.gameObject.SetActive(showGoToUpgradesButton);

        distortion.gameObject.SetActive(true);
        distortion.ResetView();
        yield return distortion.DoFade(levelTargetEnd, fadeType);
        Toggle(true, save.level);

        buttonStartNewRoundText.text = G.PlayLevelAgainFader.Contains(fadeType) ? G.I.localization.Get("play_level_again") : $"{G.I.localization.Get("play_next_level")} {save.level + 1}";

        saveInfo.Show();
        saveInfo.FadeOut(2,1);
        
        buttonEndRound.interactable = true;
        if (showNextRound) buttonStartNewRound.interactable = true;

        yield return ShowRewardAnim(rewardCollected, 0, energyOnStart, rewardTextCoin);
        yield return ShowRewardAnim(bossReward, rewardCollected, energyOnStart+rewardCollected, rewardBossTextCoin);
    }
    
    private IEnumerator ShowRewardAnim(int reward, float startRewardForRound, float startRewardForGame, Text textToDecrease)
    {
        if (reward > 0)
        {
            yield return ShowCoinAnim(reward, startRewardForRound, startRewardForGame, textToDecrease);
        }
        else if (textToDecrease == rewardTextCoin) // Только для обычной награды обновляем energyText
        {
            totalTextCoin.text = $"{NumberFormatter.Format(reward, 1)}";
            totalGameTextCoin.text = $"{NumberFormatter.Format(startRewardForGame, 1)}";
        }
    }

    private IEnumerator ShowCoinAnim(int energy, float totalEnergy,float gameEnergy, Text textToDecrease)
    {
        totalTextCoin.text = $"{NumberFormatter.Format(totalEnergy, 1)}";
        totalGameTextCoin.text = $"{NumberFormatter.Format(totalEnergy+gameEnergy, 1)}";

        yield return new WaitForSeconds(waitBeforeAnim);
        
        float addTemp = energy;
        float stepTime = animationTime / intervalNum;
        float energyPerStep = energy / intervalNum;
        float currentTotal = 0;
        float currentTotalGame = gameEnergy;
        
        for (int i = 0; i < intervalNum; i++)
        {
            yield return new WaitForSeconds(stepTime);
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

    private void StopShowCoroutine()
    {
        if (_showCoroutine == null) 
            return;
        
        StopCoroutine(_showCoroutine);
        _showCoroutine = null;
    }

    private void OnRateMeClicked()
    {
        if (rateMeData != null) MediaUrlOpener.OpenURL(rateMeData.rateURL);
    }
}