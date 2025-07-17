using UnityEngine;
using UnityEngine.UI;

public class LevelStringManager : MonoBehaviour
{
    public Text targetText;
    
    public void OnFirstLoad()
    {
        targetText.gameObject.SetActive(false);
        targetText.text = string.Empty;
    }
    
    public void OnStartRound()
    {
        targetText.gameObject.SetActive(true);
        var level = G.I.SaveProvider.Get().level;
        targetText.text = GetStringForLevel(level);
    }

    private string GetStringForLevel(int level)
    {
        if (G.I.LevelDataConfig.Count() == 0 )
            return string.Empty;
        
        var str = G.I.LevelDataConfig.Get(level)?.LevelStringEntry ?? string.Empty;
        return str.ReplaceEscapedNewlines();
    }

    public void OnEndRound()
    {
        targetText.gameObject.SetActive(false);
    }
} 