using System;
using UnityEngine;

public class PrestigeManager : MonoBehaviour
{
    public PrestigeBar prestigeBar;
    public GameObject view;

    private void Toggle(bool state)
    {
        view.gameObject.SetActive(state);
        prestigeBar.gameObject.SetActive(state);
    }
    
    [Header("Renderer Settings")]
    [SerializeField] private Renderer targetRenderer;
    
    private MaterialPropertyBlock _propertyBlock;
    private static readonly int ColorProperty = Shader.PropertyToID("_OverlayColor");
    public Gradient gradient; // градиент для цвета престижа
    public int maxPrestiges = 1000;
    public int delim = 100;

    [Header("Prestige Progression Settings")]
    [SerializeField] private float basePrestigeTime = 30f;
    [SerializeField] private float prestigeTimeGrowth = 1.08f;
    [SerializeField] private float baseBossHp = 1000f;
    [SerializeField] private float bossHpGrowth = 1.15f;
    public int RoundPoints { get; private set; }
    private DataSave _save;

    private float _currentTime;
    private float _prestigeTime;
    private bool _active;

    public void OnLoadInit()
    {
        Toggle(false);
        
        _propertyBlock = new MaterialPropertyBlock();
        targetRenderer.GetPropertyBlock(_propertyBlock);
        ApplyPrestigeColor();
    }
    
    public void StartRound()
    {
        Toggle(true);
        _save = G.I.SaveProvider.Get();
        _currentTime = 0;
        _prestigeTime = GetTime();
        _active = true;
        prestigeBar.Fill(_currentTime, _prestigeTime);
    }
    
    public void ApplyPrestigeColor()
    {
        _propertyBlock.SetColor(ColorProperty, GetCurrentPrestigeColor());
        targetRenderer.SetPropertyBlock(_propertyBlock);
    }
    
    private Color GetCurrentPrestigeColor()
    {
        var prestige = G.I.SaveProvider.Get().level;
        
        var prestigePosition = (prestige % delim) *1f / delim;
        var color = gradient.Evaluate(Mathf.Clamp01(prestigePosition));
        
        var stepIndex = (float) Math.Round(prestige*1f / maxPrestiges, 2);
        var value = 0.75f + 0.25f * stepIndex;
        color *= value;
        color.a = 1;
        
        // Debug.Log($"prestige={prestige}, posIndex={posIndex}, prestigePosition={prestigePosition}, stepIndex={stepIndex}, value={value}");
        return color;
    }

    public void Inc()
    {
        IncreasePrestige(1);
        RoundPoints = 1;
        SaveSystem.Save();
    }

    public void IncreasePrestige(int delta = 1)
    {
        var saveData = G.I.SaveProvider.Get();
        saveData.level+=delta;
        saveData.level = Mathf.Max(saveData.level, 0);
    }

    public float GetTime()
    {
        int prestige = G.I.SaveProvider.Get().level;
        return (int)( basePrestigeTime * Mathf.Pow(prestigeTimeGrowth, prestige));
    }
    
    public float GetBossHp(int prestigeLevel) => baseBossHp * Mathf.Pow(bossHpGrowth, prestigeLevel);

    public void OnEndRound() => _save.totalPrestigePoints += RoundPoints;

    public void StopRound()
    {
        Toggle(false);
        _active = false;
    }

    private void Update()
    {
        if (!_active)
            return;

        _currentTime += Time.deltaTime;

        if (_currentTime >= _prestigeTime)
        {
            _currentTime = _prestigeTime;
            _active = false;
            OnEnd();
        }

        if (!_active)
            return;

        prestigeBar.Fill(_currentTime, _prestigeTime);
    }

    private void OnEnd()
    {
        Debug.Log("call boss");
        prestigeBar.gameObject.SetActive(false);
        Inc();
        _active = false;
    }

    public void ToggleActive(bool state)
    {
        _active = state;
    }
} 