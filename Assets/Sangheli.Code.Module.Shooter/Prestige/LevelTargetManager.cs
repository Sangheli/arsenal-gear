using UnityEngine;

public class LevelTargetManager : MonoBehaviour
{
    public LevelTargetBar levelTargetBar;
    public GameObject view;
    public bool doNotApplyColor;

    [Header("Renderer Settings")]
    [SerializeField] private Renderer targetRenderer;
    
    private MaterialPropertyBlock _propertyBlock;
    private static readonly int ColorProperty = Shader.PropertyToID("_OverlayColor");
    public LevelGradientColor levelGradientColor;
    
    private DataSave _save;
    public LevelTargetValue Data;

    public void OnLoadInit()
    {
        Data ??= new LevelTargetValue(levelTargetBar,levelGradientColor);
        Toggle(false);
        
        _propertyBlock = new MaterialPropertyBlock();
        targetRenderer.GetPropertyBlock(_propertyBlock);
        ApplyLevelColor();
    }
    
    private void Toggle(bool state)
    {
        view.gameObject.SetActive(state);
    }

    public void OnStartRound()
    {
        Toggle(true);
        _save = G.I.SaveProvider.Get();
        Data.Init(G.I.currentLevel);
        levelTargetBar.Init(G.I.currentLevel, Data);
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

    public void StopRound()
    {
        Toggle(false);
        Data.Active = false;
    }

    private void Update()
    {
        if (Data != null && Data.Active) Data.AddTimeValue();
    }
} 