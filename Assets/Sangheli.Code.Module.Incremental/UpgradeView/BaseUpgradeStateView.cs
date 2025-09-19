using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseUpgradeStateView : MonoBehaviour
{
    [SerializeField] protected Text _paramNameText;
    [SerializeField] protected Text _valueText;
    [SerializeField] protected Text _levelText;
    [SerializeField] protected Text _priceText;
    [SerializeField] protected Button _upgradeButton;
    [SerializeField] protected Image _progressFill;

    protected Action RefreshMoneyCallback;
    protected Color _normalColor;
    public Color disabledColor;
    public Color goldColor = new Color(1f, 0.84f, 0f, 1f); // Gold color for maxed upgrades

    public bool IsDisabled{ get; protected set; }

    protected virtual void Awake() => _normalColor = _upgradeButton.image.color;

    public abstract void OnUpgradeClicked();
    public abstract void Refresh(bool hardcodeHide);

    protected virtual void UpdateButtonState(bool isMax, float price, bool  hardcodeHide)
    {
        IsDisabled = !(!isMax && G.I.EnergyManager.CanBuyUpgrade(price)) || hardcodeHide;
        _upgradeButton.image.color = GetColor(isMax, IsDisabled);
        _upgradeButton.enabled = GetButtonState(isMax, IsDisabled);
    }

    protected Color GetColor(bool isMax, bool isDisabled) => isMax ? goldColor : isDisabled ? disabledColor : _normalColor;
    protected static bool GetButtonState(bool isMax, bool isDisabled) => !isMax && !isDisabled;

    private void UpdatePriceText(bool isMax, uint price) => 
        _priceText.text = isMax ? G.I.localization.Get("max") : $"{G.I.localization.Get("price")}: {NumberFormatter.FormatAuto(price)}";

    protected void RefreshView(string paramName, float value, float? nextValue, int bought, int max, bool isMax, uint price, float maxValue, float baseValue, bool  hardcodeHide, bool useAutoFormat)
    {
        _paramNameText.text = paramName;
        if (isMax || nextValue == null)
            _valueText.text = useAutoFormat ? $"{NumberFormatter.FormatAuto(value)}" : $"{NumberFormatter.Format(value)}";
        else
            _valueText.text = useAutoFormat ? $"{NumberFormatter.FormatAuto(value)} → {NumberFormatter.FormatAuto(nextValue.Value)}" : $"{NumberFormatter.Format(value)} → {NumberFormatter.Format(nextValue.Value)}";

        _progressFill.fillAmount = (value-baseValue)/(maxValue-baseValue);
        UpdateButtonState(isMax, price, hardcodeHide);
        UpdatePriceText(isMax, price);
    }
} 