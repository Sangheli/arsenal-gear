using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStringManager : MonoBehaviour
{
    public Text targetText;
    
    [System.Serializable]
    public class LevelStringEntry
    {
        public int id;
        public string text;
    }

    [Space]
    public List<LevelStringEntry> levelStrings;

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
        var entry = levelStrings.Find(e => e.id == level);
        if (entry == null || string.IsNullOrEmpty(entry.text))
            return string.Empty;
        
        var str = entry.text;
        return str.ReplaceEscapedNewlines();
    }

    public void OnEndRound()
    {
        targetText.gameObject.SetActive(false);
    }
} 