using System;
using UnityEngine;

public class LevelGradientColor : MonoBehaviour
{
    public Gradient gradient;
    
    public Color GetLevelColor(int level)
    {
        var prestigePosition = (level % G.I.delim) *1f / G.I.delim;
        var color = gradient.Evaluate(Mathf.Clamp01(prestigePosition));
        
        var stepIndex = (float) Math.Round(level*1f / G.I.maxLevel, 2);
        var value = 0.75f + 0.25f * stepIndex;
        color *= value;
        color.a = 1;
        
        return color;
    }
}