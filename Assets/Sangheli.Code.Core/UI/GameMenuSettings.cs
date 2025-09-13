using UnityEngine;
using UnityEngine.UI;

public class GameMenuSettings : BaseCanvas
{
    public Slider audioVolumeSlider;
    private bool _init;

    public override void Init()
    {
        AudioListener.volume = G.I.SaveProvider.GetSystemSave().volume;
        SetupUI();
    }

    private void SetupUI()
    {
        if (audioVolumeSlider != null) audioVolumeSlider.value = G.I.SaveProvider.GetSystemSave().volume;

        if (_init) return;
        _init = true;

        audioVolumeSlider.onValueChanged.AddListener(ApplyAudioVolume);
    }

    private void ApplyAudioVolume(float value)
    {
        G.I.SaveProvider.GetSystemSave().SetVolume(value);
        G.I.SaveProvider.SaveSystemSave();
        AudioListener.volume = value;
    }
}