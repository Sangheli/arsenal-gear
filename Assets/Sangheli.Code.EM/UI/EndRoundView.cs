using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndRoundView : MonoBehaviour
{
    public GameObject View;
    public Button buttonEndRound;
    public Button buttonStartNewRound;

    public Text energyText;
    public float animationTime = 1;
    public float intervalNum = 10;
    
    public Text buttonStartNewRoundText;
    public EndRoundViewDistortion distortion;
    
    [Space]
    // Параметры баунс анимации
    public float waitBeforeAnim = 0.5f;

    [Space] 
    public Text rewardText;
    public Text rewardBossText;
    
    private Color _buttonStartNewRoundOriginalColor;
    private Color _buttonStartNewRoundTextOriginalColor;

    
    private const string PrefixRound = "Round reward: ";
    private const string PrefixBoss = "Boss reward: ";
    private const string PrefixTotal = "Total: ";
    
    private Coroutine _showCoroutine;

    public SaveInfo saveInfo;
    
    private void OnValidate()
    {
        buttonStartNewRoundText ??= buttonStartNewRound.GetComponentInChildren<Text>();
    }

    private void Awake()
    {
        _buttonStartNewRoundOriginalColor = buttonStartNewRound.image.color;
        _buttonStartNewRoundTextOriginalColor = buttonStartNewRoundText.color;
        buttonEndRound.onClick.AddListener(OnExitFromRound);
        buttonStartNewRound.onClick.AddListener(OnStartNewRound);
        Toggle(false);
        
        buttonStartNewRound.image.color = Color.clear;
        buttonStartNewRoundText.color = Color.clear;
    }

    public void OnStartRound() => distortion.gameObject.SetActive(false);

    private void OnExitFromRound()
    {
        StopShowCoroutine();
        G.I.GoToUpgrades();
    }

    private void OnStartNewRound()
    {
        StopShowCoroutine();
        G.I.StartRun();
    }

    public void Toggle(bool state)
    {
        buttonEndRound.interactable = false;
        buttonStartNewRound.interactable = false;
        buttonStartNewRound.interactable = false;
        View.SetActive(state);
    }

    public void Show(int energy, int totalEnergy, int bossReward, EndRoundFadeEnum fadeType)
    {
        StopShowCoroutine();
        _showCoroutine = StartCoroutine(ShowCoroutine(energy, totalEnergy, bossReward, fadeType));
    }

    private IEnumerator ShowCoroutine(int energy, int totalEnergy,int bossReward, EndRoundFadeEnum fadeType)
    {
        buttonStartNewRound.image.color = Color.clear;
        buttonStartNewRoundText.color = Color.clear;
        
        rewardText.text = $"{PrefixRound}{NumberFormatter.Format(energy)}";
        rewardBossText.text = $"{PrefixBoss}{NumberFormatter.Format(bossReward)}";

        var save = G.I.SaveProvider.Get();
        var showGoToUpgradesButton = true;
        var showNextRound = true;

        if (save != null && save.level < G.I.levelDataConfig.Count())
        {
            var levelId = fadeType == EndRoundFadeEnum.Win ? save.level - 1 : save.level;
            var levelConfig = G.I.levelDataConfig.Get(levelId);
            if (levelConfig != null)
            {
                showGoToUpgradesButton = levelConfig.showGoToUpgradesButton;
                if (showGoToUpgradesButton && !save.hasVisitedUpgradeMenu) showNextRound = false;
            }
        }

        if (showNextRound)
        {
            buttonStartNewRound.image.color = _buttonStartNewRoundOriginalColor;
            buttonStartNewRoundText.color = _buttonStartNewRoundTextOriginalColor;
        }

        buttonEndRound.gameObject.SetActive(showGoToUpgradesButton);

        distortion.gameObject.SetActive(true);
        distortion.ResetView();
        yield return distortion.DoFade(fadeType);
        Toggle(true);

        buttonStartNewRoundText.text = fadeType == EndRoundFadeEnum.Fail ? "Play Level Again" : $"Play next level: {save.level+1}";

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
            energyText.text = $"{PrefixTotal}{NumberFormatter.Format(reward + totalEnergy)}";
        }
    }

    private IEnumerator ShowCoinAnim(int energy, float totalEnergy, string prefix, Text textToDecrease)
    {
        energyText.text = $"{PrefixTotal}{NumberFormatter.Format(totalEnergy)}";

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
        textToDecrease.text = $"{prefix}{NumberFormatter.Format(Mathf.Max(addTemp, 0))}";
        energyText.text = $"{PrefixTotal}{NumberFormatter.Format(currentTotal)}";
    }

    private void StopShowCoroutine()
    {
        if (_showCoroutine == null) 
            return;
        
        StopCoroutine(_showCoroutine);
        _showCoroutine = null;
    }
}