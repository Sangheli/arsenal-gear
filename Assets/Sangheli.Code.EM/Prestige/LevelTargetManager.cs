using UnityEngine;
using UnityEngine.Serialization;

public class LevelTargetManager : MonoBehaviour
{
    public LevelTargetBar levelTargetBar;
    public GameObject view;
    public bool doNotApplyColor;

    [Header("Renderer Settings")]
    [SerializeField] private Renderer targetRenderer;
    
    private MaterialPropertyBlock _propertyBlock;
    private static readonly int ColorProperty = Shader.PropertyToID("_OverlayColor");
    [FormerlySerializedAs("LevelGradientColor")] public LevelGradientColor levelGradientColor;
    
    private DataSave _save;
    private LevelConfig _currentLevelConfig;
    public LevelTargetValue Data;

    private void Awake()
    {
        Data ??= new LevelTargetValue(levelTargetBar,levelGradientColor);
    }

    private void Toggle(bool state)
    {
        view.gameObject.SetActive(state);
    }

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
        _currentLevelConfig = G.I.LevelDataConfig.Get(_save.level);
        Data.Init(_currentLevelConfig, GetTime());
        levelTargetBar.Init(_currentLevelConfig, Data);
        levelTargetBar.UpdateView();
    }
    
    public void ApplyLevelColor()
    {
        if(doNotApplyColor)
            return;
        
        _propertyBlock.SetColor(ColorProperty, GetCurrentLevelColor());
        targetRenderer.SetPropertyBlock(_propertyBlock);
    }
    
    private Color GetCurrentLevelColor()
    {
        var level = G.I.SaveProvider.Get().level;
        return levelGradientColor.GetLevelColor(level);
    }

    private float GetTime()
    {
        var level = _save.level;
        return G.I.LevelDataConfig.Get(level).roundTime;
    }
    
    public void StopRound()
    {
        Toggle(false);
        Data.Active = false;
    }

    private void Update()
    {
        if(Data.Active) Data.AddTimeValue();
    }
} 