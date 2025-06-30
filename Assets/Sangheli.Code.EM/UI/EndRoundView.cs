using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndRoundView : MonoBehaviour
{
    public GameObject View;
    public Button buttonEndRound;
    public Button buttonStartNewRound;
    public Action Callback;

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
    private bool _colorsInitialized;
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
        _colorsInitialized = true;
        buttonEndRound.onClick.AddListener(OnExitFromRound);
        buttonStartNewRound.onClick.AddListener(OnStartNewRound);
        Toggle(false);
        
        buttonStartNewRound.image.color = Color.clear;
        buttonStartNewRoundText.color = Color.clear;
    }

    private void OnExitFromRound()
    {
        StartCoroutine(ExitFromRound());
    }
    
    private void OnStartNewRound()
    {
        G.I.StartRun();
    }
    
    private IEnumerator ExitFromRound()
    {
        G.I.distortionUI.DoBlock(true);
        yield return G.I.distortionUI.DoFade(forward: true);
        Callback?.Invoke();
        Toggle(false);
        G.I.StopRun();
        yield return new WaitForSeconds(G.I.distortionUI.endRoundWaitTime);
        yield return G.I.distortionUI.DoFade(forward: false);
        G.I.distortionUI.DoBlock(false);
    }

    public void Toggle(bool state)
    {
        buttonEndRound.interactable = false;
        buttonStartNewRound.interactable = false;
        buttonStartNewRound.interactable = false;
        View.SetActive(state);
    }

    public void Show()
    {
        var save = G.I.SaveProvider.Get();
        if (save.hasVisitedUpgradeMenu)
        {
            buttonStartNewRound.image.color = _buttonStartNewRoundOriginalColor;
            buttonStartNewRoundText.color = _buttonStartNewRoundTextOriginalColor;
        }

        checkPointNotification.gameObject.SetActive(false);
        Toggle(true);

        var energy = G.I.EnergyManager.RoundEnergyCount;
        var totalEnergy = save.totalEnergy;

        var prestigeDelta = G.I.prestigeManager.RoundPoints;
        
		prestigePointsText.text = $"P Points: +{prestigeDelta}";

        SaveSystem.Save();
        ShowCheckpointNotification();
        buttonEndRound.interactable = true;
        if ( save.hasVisitedUpgradeMenu) buttonStartNewRound.interactable = true;
        if (energy > 0) StartCoroutine(ShowCoinAnim(totalEnergy, energy));
    }
    
    private IEnumerator ShowCoinAnim(int startTotal, int addedEnergy)
    {
        float finalTotal = startTotal + addedEnergy;
        
        energyText.text = $"Add {NumberFormatter.Format(addedEnergy)}: {NumberFormatter.Format(addedEnergy)} -> {NumberFormatter.Format(startTotal)}";

        yield return new WaitForSeconds(waitBeforeAnim);
        
        float addTemp = addedEnergy;
        
        float stepTime = animationTime / intervalNum;
        float energyPerStep = addedEnergy / intervalNum;

        float currentTotal = startTotal;
        for (int i = 0; i < intervalNum; i++)
        {
            currentTotal += energyPerStep;
            addTemp -=  energyPerStep;
            energyText.text = $"Add {NumberFormatter.Format(addedEnergy)}: {NumberFormatter.Format(addTemp)} -> {NumberFormatter.Format(currentTotal)}";
            G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.collect2);
            yield return new WaitForSeconds(stepTime);
        }
        
        energyText.text = $"Add {NumberFormatter.Format(addedEnergy)}: {NumberFormatter.Format(0)} -> {NumberFormatter.Format(finalTotal)}";
    }
    
    private void ShowCheckpointNotification()
    {
        checkPointNotification.gameObject.SetActive(true);
        checkPointNotification.transform.localScale = Vector3.zero;
        StartCoroutine(GameAnimation.Bounce(checkPointNotification.transform, Vector3.one, bounceScale,bounceDuration));
    }
}