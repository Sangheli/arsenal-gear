using UnityEngine;
using UnityEngine.UI;

public class UpgradeParamRow : MonoBehaviour
{
    public Text displayNameText;
    public InputField basePriceInput;
    [HideInInspector] public string ParamKey;

    public void Set(string key, string displayName, float basePrice)
    {
        ParamKey = key;
        displayNameText.text = displayName;
        basePriceInput.text = basePrice.ToString();
    }

    public float GetBasePrice()
    {
        float.TryParse(basePriceInput.text, out float value);
        return value;
    }

    public void SetBasePrice(float value)
    {
        basePriceInput.text = value.ToString();
    }
} 