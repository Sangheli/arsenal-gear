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
    public AudioSettingsSangheli laserShoot;
    [FormerlySerializedAs("hit2")] public AudioSettingsSangheli emptyHit;
    
    [Space]
    [FormerlySerializedAs("death")]
    public AudioSettingsSangheli enemyDeathSound;
    public AudioSettingsSangheli enemyDeathSoundRobot;
    
    [Space,Header("Hit")]
    public AudioSettingsSangheli enemyMetalHit;
    
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
    
    [Space]
    public AudioSettingsSangheli shell;

    [Serializable]
    public class AudioSettingsSangheli
    {
        public List<AudioClip> ClipList;
        public float PitchRange = 0.25f;
        public float Volume = 0.2f;
        public bool useCustomPitch;
        public bool useCustomStartPitch;
        public float customStartPitch = 1;
        
        public float GetPitch()
        {
            if (useCustomPitch)
            {
                return PitchRange;
            }
            else if (useCustomStartPitch)
            {
                return customStartPitch + UnityEngine.Random.Range(-PitchRange, PitchRange);

            }
            else
            {
                return 1f + UnityEngine.Random.Range(-PitchRange, PitchRange);
            }
        }
    }
}
