using UnityEngine;

[CreateAssetMenu(fileName = "ColorContainer", menuName = "ScriptableObjects/ColorContainer", order = 1)]
public class ColorContainer : ScriptableObject
{
    [SerializeField] 
    private Color[] colors = new Color[0];

    public Color GetColor(int index)
    {
        if (index < 0 || index >= colors.Length)
        {
            Debug.LogWarning($"Color index out of range: {index}. Returning white color.");
            return Color.white;
        }
        
        return colors[index];
    }
    
    public int Length => colors.Length;
    
    public Color this[int index] => GetColor(index);
}