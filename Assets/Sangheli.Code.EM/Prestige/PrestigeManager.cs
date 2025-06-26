using System;
using UnityEngine;

public class PrestigeManager : MonoBehaviour
{
    [Header("Prestige Colors")] 
    [SerializeField]
    public PrestigeData[] prestigeData;
    
    [Header("Renderer Settings")]
    [SerializeField] private Renderer targetRenderer;
    
    private MaterialPropertyBlock _propertyBlock;
    private static readonly int ColorProperty = Shader.PropertyToID("_OverlayColor");
    public Gradient gradient; // градиент для цвета престижа
    public int maxPrestiges = 1000;
    public int delim = 100;

    public void OnLoadInit()
    {
        _propertyBlock = new MaterialPropertyBlock();
        targetRenderer.GetPropertyBlock(_propertyBlock);

        for (var index = 0; index < prestigeData.Length; index++)
        {
            var x = prestigeData[index];
            x.Index = index;
        }
        
        ApplyPrestigeColor();
    }
    
    public void ApplyPrestigeColor()
    {
        _propertyBlock.SetColor(ColorProperty, GetCurrentPrestigeColor());
        targetRenderer.SetPropertyBlock(_propertyBlock);
    }
    
    public Color GetCurrentPrestigeColor()
    {
        var prestige = G.I.SaveProvider.Get().prestige;
        
        var prestigePosition = (prestige % delim) *1f / delim;
        var color = gradient.Evaluate(Mathf.Clamp01(prestigePosition));
        
        var stepIndex = (float) Math.Round(prestige*1f / maxPrestiges, 2);
        var value = 0.5f + 0.5f * stepIndex;
        color *= value;
        color.a = 1;
        
        // Debug.Log($"prestige={prestige}, posIndex={posIndex}, prestigePosition={prestigePosition}, stepIndex={stepIndex}, value={value}");
        return color;
    }

    public void IncreasePrestige(int delta = 1)
    {
        var saveData = G.I.SaveProvider.Get();
        saveData.prestige+=delta;
        saveData.prestige = Mathf.Max(saveData.prestige, 0);
        SaveSystem.Save();
        ApplyPrestigeColor();
    }

    public PrestigeData GetLastPrestige(int prestigeIndex)
    {
        var index = Mathf.Min(G.I.prestigeManager.prestigeData.Length - 1, prestigeIndex);
        return G.I.prestigeManager.prestigeData[index];
    }
} 