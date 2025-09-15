using System.Collections.Generic;

public static class PitchHelper
{
    private static readonly Dictionary<string, int> ClickCounts = new();
    private const int MaxClicks = 8;
    private const float MinPitch = 0.5f;
    private const float MaxPitch = 1.0f;
    private const float PitchStep = (MaxPitch - MinPitch) / MaxClicks;

    public static float GetAndIncrement(string key)
    {
        if (!ClickCounts.ContainsKey(key)) ClickCounts[key] = 0;
        var count = ClickCounts[key];
        var pitch = MaxPitch - PitchStep * count;
        if (count < MaxClicks) ClickCounts[key] = count + 1;
        return pitch < MinPitch ? MinPitch : pitch;
    }

    public static void ResetAll() => ClickCounts.Clear();
} 