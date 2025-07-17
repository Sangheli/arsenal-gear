using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Sangheli/SoundContainer")]
public class SoundContainer : ScriptableObject
{
    public AudioSettingsSangheli buttonClick;
    
    [Space]
    public AudioSettingsSangheli hit;
    [FormerlySerializedAs("hit2")] public AudioSettingsSangheli emptyHit;
    
    [Space]
    [FormerlySerializedAs("collect")] public AudioSettingsSangheli collectCoin;
    [FormerlySerializedAs("collect2")] public AudioSettingsSangheli ShowCoinAnim;

    [FormerlySerializedAs("woosh1")] [Space]
    public AudioSettingsSangheli loadinScreen;

    [Space]
    public AudioSettingsSangheli bossHit;
    public AudioSettingsSangheli bossDeath;
    
    [Space]
    public AudioSettingsSangheli gameOver;
    
    [Serializable]
    public class AudioSettingsSangheli
    {
        public List<AudioClip> ClipList;
        public float PitchRange = 0.25f;
        public float Volume = 0.2f;
    }
}
