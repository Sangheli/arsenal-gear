using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShieldBarUI : MonoBehaviour
{
    public Image shieldImage;
    public Text shieldText;
    public Image[] shieldAllRender;
    public List<Color> shieldColors;
    public Color shieldTextColor;
    public Color blinkColor;
    public float blinkTime = 0.05f;
    public AnimationCurve blinkCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private Color _startColor;
    private Coroutine _blinkCoroutine;
    private DataSave _save;
    
    private void Awake()
    {
        if (shieldImage != null)
            _startColor = shieldImage.color;
        if (shieldColors == null || shieldColors.Count == 0)
        {
            shieldColors = new List<Color>();
            foreach (var x in shieldAllRender) shieldColors.Add(x.color);
        }
        shieldTextColor = shieldText.color;
    }

    public void Init()
    {
        _save = G.I.SaveProvider.Get();
        if (shieldImage != null)
            shieldImage.color = _startColor;
    }

    public void ToggleShieldView(bool available)
    {
        for (var index = 0; index < shieldAllRender.Length; index++)
        {
            var x = shieldAllRender[index];
            x.color = available ? shieldColors[index] : Color.clear;
        }

        shieldText.color = available ? shieldTextColor : Color.clear;

        if (available && shieldImage != null)
            shieldImage.color = _startColor;
    }

    public bool ShowShield(float hp, float fullValue, float fillValue, bool doBlink)
    {
        if (!gameObject.activeSelf)
            return false;

        if (!G.I.ShieldManager.IsShieldAvailable())
            return false;

        if (shieldImage != null)
            shieldImage.fillAmount = fillValue;
        if (shieldText != null)
            shieldText.text = $"{NumberFormatter.Format(hp, 2)}/{NumberFormatter.Format(fullValue, 2)}";

        if (doBlink && shieldImage != null)
        {
            if (_blinkCoroutine != null)
                StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = StartCoroutine(GameAnimation.BlinkColor(blinkColor, _startColor, shieldImage, blinkTime, blinkCurve));
        }

        return true;
    }
} 