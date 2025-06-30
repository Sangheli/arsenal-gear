using System.Collections.Generic;

public static class UpgradePitchHelper
{
    private static Dictionary<string, int> _clickCounts = new();
    private const int MaxClicks = 8;
    private const float MinPitch = 0.5f;
    private const float MaxPitch = 1.0f;
    private const float PitchStep = (MaxPitch - MinPitch) / MaxClicks;

    public static float GetAndIncrement(string key)
    {
        if (!_clickCounts.ContainsKey(key))
            _clickCounts[key] = 0;
        int count = _clickCounts[key];
        float pitch = MaxPitch - PitchStep * count;
        if (count < MaxClicks)
            _clickCounts[key] = count + 1;
        return pitch < MinPitch ? MinPitch : pitch;
    }

    public static void ResetAll()
    {
        _clickCounts.Clear();
    }
} 