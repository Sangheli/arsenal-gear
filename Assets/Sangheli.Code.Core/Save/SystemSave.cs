using System;

[Serializable]
public class SystemSave
{
    public bool mute;
    public float volume = 1;
    public void SetVolume(float value) => volume = value;
    public void ToggleMute() => mute = !mute;
}