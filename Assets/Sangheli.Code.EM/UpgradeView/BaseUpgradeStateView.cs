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
    private Color _normalColor;
    public Color disabledColor;

    public bool IsDisabled{ get; private set; }

    protected virtual void Awake() => _normalColor = _upgradeButton.image.color;

    public abstract void OnUpgradeClicked();
    public abstract void Refresh();

    private void UpdateButtonState(bool isMax, float price)
    {
        IsDisabled = !(!isMax && G.I.EnergyManager.CanBuyUpgrade(price));
        _upgradeButton.image.color = IsDisabled ? disabledColor : _normalColor;
        _upgradeButton.enabled = !IsDisabled;
    }

    private void UpdatePriceText(bool isMax, int price) => 
        _priceText.text = isMax ? G.I.localization.Get("max") : $"{G.I.localization.Get("price")}: {NumberFormatter.Format(price, 2)}";

    protected void RefreshView(string paramName, float value, float? nextValue, int bought, int max, bool isMax, int price, float maxValue, float baseValue)
    {
        _paramNameText.text = paramName;
        if (isMax || nextValue == null)
            _valueText.text = $"{NumberFormatter.Format(value)}";
        else
            _valueText.text = $"{NumberFormatter.Format(value)} => {NumberFormatter.Format(nextValue.Value)}";
        _progressFill.fillAmount = (value-baseValue)/(maxValue-baseValue);
        UpdateButtonState(isMax, price);
        UpdatePriceText(isMax, price);
    }
} 