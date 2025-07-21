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

    private DataSave _save;

    private float _currentTime;
    private float _prestigeTime;
    private bool _active;

    public bool IsTimerOver => _currentTime >= _prestigeTime;

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
        var prestigePosition = (level % G.I.delim) *1f / G.I.delim;
        var color = gradient.Evaluate(Mathf.Clamp01(prestigePosition));
        
        var stepIndex = (float) Math.Round(level*1f / G.I.maxLevel, 2);
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

    private float GetTime()
    {
        var level = _save.level;
        return G.I.LevelDataConfig.Get(level).roundTime;
    }
    
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
        prestigeBar.gameObject.SetActive(false);
        var level = G.I.SaveProvider.Get().level;
        // Проверка флага showBossAfterTimer
        var levelConfig = G.I.LevelDataConfig.Count() > level ? G.I.LevelDataConfig.Get(level) : null;
        if (levelConfig != null && !levelConfig.showBossAfterTimer)
        {
            CompletePrestigeAfterBoss(); // Сразу победа
            return;
        }
        float bossHp = levelConfig?.bossHp ?? 1000f;
        Color bossColor = GetLevelColor(level + 1);
        float scale = 9f;
        float damage = levelConfig?.bossAttack ?? 1;
        _active = false;
        G.I.tutorial.OnEndRound(); // Скрываем туториал при появлении босса
        G.I.bossManager.SpawnBoss(bossHp, bossColor, scale,damage, CompletePrestigeAfterBoss);
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
        var levelConfig = G.I.LevelDataConfig.Get(level);
        return levelConfig?.bossReward ?? 100;
    }

    public float GetLevelTimePercent() => 
        _prestigeTime <= 0f ? 0f : Mathf.Clamp01(_currentTime / _prestigeTime);
} 