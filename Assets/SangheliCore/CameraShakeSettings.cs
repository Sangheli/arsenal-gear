using UnityEngine;

[CreateAssetMenu(fileName = "CameraShakeSettings", menuName = "Sangheli/Camera Shake Settings")]
public class CameraShakeSettings : ScriptableObject
{
    [Header("General Shake Settings")]
    public float shakeIntensity = 0.3f;
    public float shakeDuration = 0.15f;
    public float shakeFrequency = 80f;
    public AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    [Header("Death Shake Settings")]
    public float deathShakeIntensity = 0.8f;
    public float deathShakeDuration = 0.25f;
    public float deathShakeFrequency = 120f;
} 