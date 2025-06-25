using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    public Text energyText;

    public void ShowEnergy(int value)
    {
        if (energyText != null)
            energyText.text = $"{NumberFormatter.Format(value, 0)}e";
    }
} 