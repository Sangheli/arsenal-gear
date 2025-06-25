using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class BaseUpgradeStateView : MonoBehaviour
{
    [SerializeField] protected Text _paramNameText;
    [SerializeField] protected Text _valueText;
    [SerializeField] protected Text _levelText;
    [SerializeField] protected Text _priceText;
    [SerializeField] protected Button _upgradeButton;

    protected Action RefreshMoneyCallback;
    private Color _normalColor;
    public Color disabledColor;

    public bool IsDisabled{ get; private set; }
    public int MinPrestige { get; protected set; }

    protected virtual void Awake() => _normalColor = _upgradeButton.image.color;

    public abstract void OnUpgradeClicked();
    public abstract void Refresh();

    private void UpdateButtonState(bool isMax, float price)
    {
        IsDisabled = !(!isMax && G.I.EnergyManager.CanAffordUpgrade(price));
        _upgradeButton.image.color = IsDisabled ? disabledColor : _normalColor;
        _upgradeButton.enabled = !IsDisabled;
    }

    private void UpdatePriceText(bool isMax, float price) => 
        _priceText.text = isMax ? "Max" : $"Price: {NumberFormatter.Format(price, 0)}";

    protected void RefreshView(string paramName, float value, float? nextValue, int bought, int max, bool isMax, float price)
    {
        _paramNameText.text = paramName;
        if (isMax || nextValue == null)
            _valueText.text = $"{NumberFormatter.Format(value)}";
        else
            _valueText.text = $"{NumberFormatter.Format(value)} => {NumberFormatter.Format(nextValue.Value)}";
        _levelText.text = $"Level: {bought}/{max}";
        UpdateButtonState(isMax, price);
        UpdatePriceText(isMax, price);
    }
} 