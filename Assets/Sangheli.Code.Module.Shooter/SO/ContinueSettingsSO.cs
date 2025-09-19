using UnityEngine;

[CreateAssetMenu(fileName = "ContinueSettings", menuName = "Sangheli/ContinueSettings")]
public class ContinueSettingsSO : ScriptableObject
{
    [Tooltip("Percentage of health to restore when continuing (0-100)")]
    public float healthRestorePercent = 50f;
    
    [Tooltip("Enable or disable the continue feature")]
    public bool continueEnabled = true;
    
    [Tooltip("Allow only one continue per game session")]
    public bool oneContinuePerSession = true;
    
    [Tooltip("Delay in seconds before enemies can attack after respawn")]
    public float enemyAttackDelay = 1.5f;
}