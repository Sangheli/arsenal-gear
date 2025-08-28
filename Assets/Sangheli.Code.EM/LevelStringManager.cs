using UnityEngine;
using UnityEngine.UI;

public class LevelStringManager : MonoBehaviour
{
    public Text targetText;
    public GameObject back;
    
    public void OnFirstLoad()
    {
        back.gameObject.SetActive(false);
        targetText.text = string.Empty;
    }
    
    public void OnStartRound()
    {
        var level = G.I.SaveProvider.Get().level;
        var zz = GetStringForLevel(level);
        targetText.text = zz;
        back.gameObject.SetActive(!string.IsNullOrWhiteSpace(zz));
    }

    private string GetStringForLevel(int level) =>
        G.I.localization
            .Get
                (G.I.LevelDataConfig.Get(level)?.LevelStringEntry ?? string.Empty)
            .ReplaceEscapedNewlines();

    public void OnEndRound()
    {
        back.gameObject.SetActive(false);
    }
} 