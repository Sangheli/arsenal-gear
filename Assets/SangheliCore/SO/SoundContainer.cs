using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sangheli/SoundContainer")]
public class SoundContainer : ScriptableObject
{
    public AudioSettingsSangheli buttonClick;
    public AudioSettingsSangheli hit;
    public AudioSettingsSangheli collect;
    public AudioSettingsSangheli collect2;
    public AudioSettingsSangheli hit2;
    public AudioSettingsSangheli woosh1;
    public AudioSettingsSangheli woosh2;
    public AudioSettingsSangheli checkpoint;
    public AudioSettingsSangheli enemyDeath;

    [Serializable]
    public class AudioSettingsSangheli
    {
        public List<AudioClip> ClipList;
        public float PitchRange = 0.25f;
        public float Volume = 0.2f;
    }
}
