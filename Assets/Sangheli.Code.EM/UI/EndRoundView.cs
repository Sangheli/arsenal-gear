using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndRoundView : MonoBehaviour
{
    //text
    public Text energyText;
    public Text rewardText;
    public Text rewardBossText;
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
    
    private Coroutine _showCoroutine;

    public SaveInfo saveInfo;

    public Image startChild;
    
    private void OnValidate()
    {
        buttonStartNewRoundText ??= buttonStartNewRound.GetComponentInChildren<Text>();
    }

    private void Awake()
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
        G.I.StartRun();
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
            upgradesText.text = G.I.localization.Get("go_to_upgrades");
            checkPointText.text = G.I.localization.Get("checkpoint_done");
            
            // Set rate me button text if the button exists
            if (rateMeText != null)
                rateMeText.text = G.I.localization.Get("rate_me");
            
            rateMeButton.gameObject.SetActive(!G.I.Data.storeConfig.DisableForStore && level>=10 && level%5==0);
        }
        
        buttonEndRound.interactable = false;
        buttonStartNewRound.interactable = false;
        buttonStartNewRound.interactable = false;
        View.SetActive(state);
        returnToStartButton.gameObject.SetActive(false);
    }

    public void Show(int energy, int totalEnergy, int bossReward, EndRoundFadeEnum fadeType, ILevelTargetEnd levelTargetEnd)
    {
        StopShowCoroutine();
        _showCoroutine = StartCoroutine(ShowCoroutine(energy, totalEnergy, bossReward, fadeType, levelTargetEnd));
    }

    private IEnumerator ShowCoroutine(int energy, int totalEnergy,int bossReward, EndRoundFadeEnum fadeType, ILevelTargetEnd levelTargetEnd)
    {
        buttonStartNewRound.image.color = Color.clear;
        buttonStartNewRoundText.color = Color.clear;
        startChild.color = Color.clear;
        
        rewardText.text = $"{G.I.localization.Get(PrefixRound)}{NumberFormatter.Format(energy)}";
        rewardBossText.text = $"{G.I.localization.Get(PrefixBoss)}{NumberFormatter.Format(bossReward)}";

        var save = G.I.SaveProvider.Get();
        var showGoToUpgradesButton = true;
        var showNextRound = true;

        if (save != null)
        {
            var levelId = fadeType == EndRoundFadeEnum.RoundWin ? save.level - 1 : save.level;
            var levelConfig = G.I.LevelDataConfig.Get(levelId);
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

        yield return ShowRewardAnim(energy, totalEnergy, PrefixRound, rewardText);
        yield return ShowRewardAnim(bossReward, totalEnergy + energy, PrefixBoss, rewardBossText);
    }
    
    private IEnumerator ShowRewardAnim(int reward, float totalEnergy, string prefix, Text textToDecrease)
    {
        if (reward > 0)
        {
            yield return ShowCoinAnim(reward, totalEnergy, prefix, textToDecrease);
        }
        else if (textToDecrease == rewardText) // Только для обычной награды обновляем energyText
        {
            energyText.text = $"{G.I.localization.Get(PrefixTotal)}{NumberFormatter.Format(reward + totalEnergy)}";
        }
    }

    private IEnumerator ShowCoinAnim(int energy, float totalEnergy, string prefix, Text textToDecrease)
    {
        energyText.text = $"{G.I.localization.Get(PrefixTotal)}{NumberFormatter.Format(totalEnergy)}";

        yield return new WaitForSeconds(waitBeforeAnim);
        
        float addTemp = energy;
        float stepTime = animationTime / intervalNum;
        float energyPerStep = energy / intervalNum;
        float currentTotal = totalEnergy;
        
        for (int i = 0; i < intervalNum; i++)
        {
            yield return new WaitForSeconds(stepTime);
            currentTotal += energyPerStep;
            addTemp -=  energyPerStep;
            PrintCoinAnim(prefix, textToDecrease, addTemp, currentTotal);
            G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.ShowCoinAnim);
        }
        
        PrintCoinAnim(prefix, textToDecrease, 0, energy+totalEnergy);
    }
    
    private void PrintCoinAnim(string prefix, Text textToDecrease, float addTemp, float currentTotal)
    {
        textToDecrease.text = $"{G.I.localization.Get(prefix)}{NumberFormatter.Format(Mathf.Max(addTemp, 0))}";
        energyText.text = $"{G.I.localization.Get(PrefixTotal)}{NumberFormatter.Format(currentTotal)}";
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