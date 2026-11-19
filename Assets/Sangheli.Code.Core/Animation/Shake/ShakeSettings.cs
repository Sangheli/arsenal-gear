using System;
using UnityEngine;

[Serializable]
public class ShakeSettings
{
    public float intensity = 30f;
    public float duration = 0.15f;
    public float frequency = 80f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
}
