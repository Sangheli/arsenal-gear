using UnityEngine;

[CreateAssetMenu(fileName = "RateMeData", menuName = "Sangheli/RateMeData")]
public class RateMeSO : ScriptableObject
{
    [Tooltip("URL to open when the Rate Me button is clicked")]
    public string rateURL = "https://example.com/rate";
}