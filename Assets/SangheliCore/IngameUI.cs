using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    public Button muteButton;
    public Text muteText;
    
    private void Awake()
    {
        muteButton.onClick.AddListener(OnMute);
    }

    private void OnMute()
    {
        if (G.I == null)
            return;

        var save = G.I.SaveProvider.GetSystemSave();
        save.ToggleMute();
        G.I.SaveProvider.SaveSystemSave();
        RestoreSave();
    }

    public void RestoreSave()
    {
        var save = G.I.SaveProvider.GetSystemSave();
        muteText.text = !save.mute ? "Sounds\nEnabled" : "Sounds\nMuted";
        AudioListener.volume = save.mute ? 0 : 1;
    }
}