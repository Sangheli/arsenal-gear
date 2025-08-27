using UnityEngine;

[CreateAssetMenu(menuName = "Sangheli/BorderColorPalette")]
public class BorderColorPalette : ScriptableObject
{
    public Color[] borderColors = { Color.white, Color.red, Color.blue, Color.green };
}