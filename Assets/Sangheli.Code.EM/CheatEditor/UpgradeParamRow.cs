using UnityEngine;
using UnityEngine.UI;

public class UpgradeParamRow : MonoBehaviour
{
    public Text displayNameText;
    public InputField basePriceInput;
    public string paramKey { get; set; }

    public void Set(string key, string displayName, float basePrice)
    {
        paramKey = key;
        displayNameText.text = displayName;
        basePriceInput.text = basePrice.ToString();
    }

    public float GetBasePrice()
    {
        float.TryParse(basePriceInput.text, out var value);
        return value;
    }

    public void SetBasePrice(float value) => basePriceInput.text = value.ToString();
} 