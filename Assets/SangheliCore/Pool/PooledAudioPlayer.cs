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
        if (IsCanPlay(clip))
            return;

        float pitch = 1f + Random.Range(-clip.PitchRange, clip.PitchRange);
        var subClip = clip.ClipList[Random.Range(0, clip.ClipList.Count)];
        PlaySound(clip, pitch, subClip.length);
    }

    public void PlayWithPitch(SoundContainer.AudioSettingsSangheli clip, float pitch)
    {
        if (IsCanPlay(clip))
            return;

        var subClip = clip.ClipList[Random.Range(0, clip.ClipList.Count)];
        PlaySound(clip, pitch, subClip.length / pitch);
    }

    private bool IsCanPlay(SoundContainer.AudioSettingsSangheli clip)
    {
        if (clip?.ClipList == null) 
            return false;

        float currentTime = Time.time;

        var subClip = clip.ClipList[Random.Range(0, clip.ClipList.Count)];
        if (lastPlayTime.TryGetValue(subClip, out float lastTime))
        {
            if (currentTime - lastTime < minIntervalBetweenPlays)
                return true;
        }

        lastPlayTime[subClip] = currentTime;
        return false;
    }

    private void PlaySound(SoundContainer.AudioSettingsSangheli clip, float pitch, float len)
    {
        AudioSource source = (availableSources.Count > 0)
            ? availableSources.Dequeue()
            : AddNewSourceToPool(); // Расширяем пул при необходимости

        GameObject obj = source.gameObject;
        obj.SetActive(true);

        var subClip = clip.ClipList[Random.Range(0, clip.ClipList.Count)];
        source.clip = subClip;
        source.pitch = pitch;
        source.volume = clip.Volume;
        source.Play();

        StartCoroutine(ReleaseAfterPlay(source, len + 0.1f));
    }

    private IEnumerator ReleaseAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        availableSources.Enqueue(source);
    }
}