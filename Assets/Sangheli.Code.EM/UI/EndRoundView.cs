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
    
    public GameObject checkPointNotification;
    
    // Параметры баунс анимации
    public float bounceDuration = 0.5f;
    public float bounceScale = 1.2f;
    public float waitBeforeAnim = 0.5f;
    
    private Color _buttonStartNewRoundOriginalColor;
    private Color _buttonStartNewRoundTextOriginalColor;
    public Text buttonStartNewRoundText;
    
    public Text prestigePointsText;
    
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

    private void OnExitFromRound() => G.I.StopRun();
    private void OnStartNewRound() => G.I.StartRun();

    public void Toggle(bool state)
    {
        buttonEndRound.interactable = false;
        buttonStartNewRound.interactable = false;
        buttonStartNewRound.interactable = false;
        View.SetActive(state);
    }

    public IEnumerator Show(int energy, int totalEnergy, int prestige)
    {
        var save = G.I.SaveProvider.Get();
        if (save.hasVisitedUpgradeMenu)
        {
            buttonStartNewRound.image.color = _buttonStartNewRoundOriginalColor;
            buttonStartNewRoundText.color = _buttonStartNewRoundTextOriginalColor;
        }

        checkPointNotification.gameObject.SetActive(false);
        Toggle(true);

        prestigePointsText.text = $"P Points: +{prestige}";

        SaveSystem.Save();
        ShowCheckpointNotification();
        buttonEndRound.interactable = true;
        if ( save.hasVisitedUpgradeMenu) buttonStartNewRound.interactable = true;
        
        if (energy > 0) 
            yield return ShowCoinAnim(totalEnergy, energy);
        
        float finalTotal = totalEnergy + energy;
        energyText.text = $"Add {NumberFormatter.Format(energy)}: {NumberFormatter.Format(0)} -> {NumberFormatter.Format(finalTotal)}";
    }
    
    private IEnumerator ShowCoinAnim(int totalEnergy, int energy)
    {
        energyText.text = $"Add {NumberFormatter.Format(energy)}: {NumberFormatter.Format(energy)} -> {NumberFormatter.Format(totalEnergy)}";

        yield return new WaitForSeconds(waitBeforeAnim);
        
        float addTemp = energy;
        
        float stepTime = animationTime / intervalNum;
        float energyPerStep = energy / intervalNum;

        float currentTotal = totalEnergy;
        for (int i = 0; i < intervalNum; i++)
        {
            currentTotal += energyPerStep;
            addTemp -=  energyPerStep;
            energyText.text = $"Add {NumberFormatter.Format(energy)}: {NumberFormatter.Format(addTemp)} -> {NumberFormatter.Format(currentTotal)}";
            G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.collect2);
            yield return new WaitForSeconds(stepTime);
        }
    }
    
    private void ShowCheckpointNotification()
    {
        checkPointNotification.gameObject.SetActive(true);
        checkPointNotification.transform.localScale = Vector3.zero;
        StartCoroutine(GameAnimation.Bounce(checkPointNotification.transform, Vector3.one, bounceScale,bounceDuration));
    }
}