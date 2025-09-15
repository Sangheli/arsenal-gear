using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorContainer", menuName = "ScriptableObjects/ColorContainer", order = 1)]
public class ColorContainer : ScriptableObject
{
    [SerializeField] 
    private Color[] colors = Array.Empty<Color>();

    private Color GetColor(int index)
    {
        if (index >= 0 && index < colors.Length) return colors[index];
        Debug.LogWarning($"Color index out of range: {index}. Returning white color.");
        return Color.white;
    }
    
    public int Length => colors.Length;
    
    public Color this[int index] => GetColor(index);
}