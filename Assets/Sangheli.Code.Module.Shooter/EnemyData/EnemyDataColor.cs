using System;
using UnityEngine;

[Serializable]
public class EnemyDataColor
{
    [Space,Header("Color")]
    public bool useRandomColor;
    
    [Space]
    public Color color = Color.white;

    [Space]
    public bool userOverrideBorderColor;
    public Color overrideBorderColor;
    
    [Space]
    public Color hitVfxColor;
    
    public Color GetColor() => useRandomColor ? GetRandomColor() : color;
    public Color GetBorderColor(int difficult)
    {
        return userOverrideBorderColor ?
            overrideBorderColor :
            useRandomColor ? GetRandomColor() : G.I.Data.borderColorPaletteEnemy[difficult];
    }

    public Color GetHitVfxColor() => hitVfxColor;
    
    public static Color GetRandomColor()
    {
        // Generate vibrant colors that fit the neon aesthetic of the game
        float hue = UnityEngine.Random.value;
        float saturation = 0.8f + UnityEngine.Random.value * 0.2f; // 0.8-1.0 for vibrant colors
        float value = 0.8f + UnityEngine.Random.value * 0.2f; // 0.8-1.0 for bright colors
        
        var color = Color.HSVToRGB(hue, saturation, value);
        color.a = 1;

        return color;
    }
}