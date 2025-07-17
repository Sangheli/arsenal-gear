using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledAudioPlayer : MonoBehaviour
{
    public int initialPoolSize = 10;
    public float minIntervalBetweenPlays = 0.1f; // Минимальное время между воспроизведениями одного и того же звука

    private Queue<AudioSource> availableSources;
    private List<AudioSource> allSources;

    // Отслеживает, когда последний раз был воспроизведён каждый AudioClip
    private Dictionary<AudioClip, float> lastPlayTime;

    void Awake()
    {
        availableSources = new Queue<AudioSource>(initialPoolSize);
        allSources = new List<AudioSource>(initialPoolSize);
        lastPlayTime = new Dictionary<AudioClip, float>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            AddNewSourceToPool();
        }
    }

    private AudioSource AddNewSourceToPool()
    {
        GameObject obj = new GameObject("PooledAudioSource_" + allSources.Count);
        obj.transform.parent = this.transform;
        AudioSource source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        obj.SetActive(false);

        availableSources.Enqueue(source);
        allSources.Add(source);
        return source;
    }

    public void PlayWithRandomPitch(SoundContainer.AudioSettingsSangheli clip)
    {
        var pitch = 1f + Random.Range(-clip.PitchRange, clip.PitchRange);
        PlayWithPitch(clip, pitch);
    }
    
    public void PlayWithRandomPitchWithVolumeMod(SoundContainer.AudioSettingsSangheli clip,float volumeMode)
    {
        var pitch = 1f + Random.Range(-clip.PitchRange, clip.PitchRange);
        PlayWithPitchWithVolume(clip, pitch, volumeMode);
    }

    public void PlayWithPitch(SoundContainer.AudioSettingsSangheli clip, float pitch)
    {
        var index = Random.Range(0, clip.ClipList.Count);
        if (!IsCanPlay(clip, index))
            return;

        var subClip = clip.ClipList[index];
        PlaySound(clip, pitch, subClip.length / pitch, index);
    }

    
    public void PlayWithPitchWithVolume(SoundContainer.AudioSettingsSangheli clip, float pitch, float volumeMod)
    {
        var index = Random.Range(0, clip.ClipList.Count);
        if (!IsCanPlay(clip, index))
            return;

        var subClip = clip.ClipList[index];
        PlaySound(clip, pitch, subClip.length / pitch, index, volumeMod);
    }
    
    private bool IsCanPlay(SoundContainer.AudioSettingsSangheli clip, int index)
    {
        if (clip?.ClipList == null) 
            return false;

        float currentTime = Time.time;

        var subClip = clip.ClipList[index];
        if (lastPlayTime.TryGetValue(subClip, out float lastTime))
        {
            if (currentTime - lastTime < minIntervalBetweenPlays)
                return false;
        }

        lastPlayTime[subClip] = currentTime;
        return true;
    }

    private void PlaySound(SoundContainer.AudioSettingsSangheli clip, float pitch, float len, int index, float volumeMod = 1)
    {
        AudioSource source = (availableSources.Count > 0)
            ? availableSources.Dequeue()
            : AddNewSourceToPool(); // Расширяем пул при необходимости

        GameObject obj = source.gameObject;
        obj.SetActive(true);

        var subClip = clip.ClipList[index];
        source.clip = subClip;
        source.pitch = pitch;
        source.volume = clip.Volume * volumeMod;
        source.Play();

        StartCoroutine(ReleaseAfterPlay(source, len + 0.1f));
    }

    private IEnumerator ReleaseAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Stop();
        source.clip = null;
        // source.gameObject.SetActive(false);
        availableSources.Enqueue(source);
    }
}