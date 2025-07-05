using System;
using UnityEngine;

public class PrestigeManager : MonoBehaviour
{
    public DoubleImageFillBar prestigeBar;
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
    [Header("Prestige Reward Settings")]
    [SerializeField] private int baseBossReward = 100;
    [SerializeField] private float bossRewardGrowth = 1.1f;
    private DataSave _save;

    private float _currentTime;
    private float _prestigeTime;
    private bool _active;

    public void OnLoadInit()
    {
        Toggle(false);
        
        _propertyBlock = new MaterialPropertyBlock();
        targetRenderer.GetPropertyBlock(_propertyBlock);
        ApplyLevelColor();
    }
    
    public void OnStartRound()
    {
        Toggle(true);
        _save = G.I.SaveProvider.Get();
        _currentTime = 0;
        _prestigeTime = GetTime();
        _active = true;
        prestigeBar.Fill(_currentTime, _prestigeTime);
    }
    
    public void ApplyLevelColor()
    {
        _propertyBlock.SetColor(ColorProperty, GetCurrentLevelColor());
        targetRenderer.SetPropertyBlock(_propertyBlock);
    }
    
    private Color GetCurrentLevelColor()
    {
        var level = G.I.SaveProvider.Get().level;
        return GetLevelColor(level);
    }
    
    private Color GetLevelColor(int level)
    {
        var prestigePosition = (level % delim) *1f / delim;
        var color = gradient.Evaluate(Mathf.Clamp01(prestigePosition));
        
        var stepIndex = (float) Math.Round(level*1f / maxPrestiges, 2);
        var value = 0.75f + 0.25f * stepIndex;
        color *= value;
        color.a = 1;
        
        return color;
    }

    public void ChangeLevel(int delta = 1)
    {
        var saveData = G.I.SaveProvider.Get();
        saveData.level+=delta;
        saveData.level = Mathf.Max(saveData.level, 0);
    }

    public float GetTime()
    {
        return basePrestigeTime;
        int level = G.I.SaveProvider.Get().level;
        return (int)( basePrestigeTime * Mathf.Pow(prestigeTimeGrowth, level));
    }
    
    public float GetBossHp(int prestigeLevel) => baseBossHp * Mathf.Pow(bossHpGrowth, prestigeLevel);

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
        var level = G.I.SaveProvider.Get().level;
        float bossHp = GetBossHp(level);
        Color bossColor = GetLevelColor(level + 1);
        float scale = 10.5f;
        _active = false;
        G.I.bossManager.SpawnBoss(bossHp, bossColor, scale, CompletePrestigeAfterBoss);
    }

    public void CompletePrestigeAfterBoss()
    {
        ChangeLevel(1);
        SaveSystem.Save();
        G.I.StopRun(EndRoundFadeEnum.Win);
    }

    public void ToggleActive(bool state)
    {
        _active = state;
    }

    public int GetBossReward(int level)
    {
        var levelFix = Mathf.Max(0, level);
        return Mathf.RoundToInt(baseBossReward * Mathf.Pow(bossRewardGrowth, levelFix));
    }
} 