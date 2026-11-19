using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sangheli/SoundContainer")]
public class SoundContainer : ScriptableObject
{
    public AudioSettingsSangheli buttonClick;
    
    [Space]
    public AudioSettingsSangheli hit;
    public AudioSettingsSangheli laserShoot;
    public AudioSettingsSangheli emptyHit;
    
    [Space]
    public AudioSettingsSangheli enemyDeathSound;
    public AudioSettingsSangheli enemyDeathSoundRobot;
    
    [Space]
    public AudioSettingsSangheli collectCoin;
    public AudioSettingsSangheli ShowCoinAnim;

    [Space]
    public AudioSettingsSangheli loadinScreen;

    [Space]
    public AudioSettingsSangheli bossDeath;
    
    [Space]
    public AudioSettingsSangheli gameOver;
    
    [Space]
    public AudioSettingsSangheli energyShieldHit;

    [Serializable]
    public class AudioSettingsSangheli
    {
        public List<AudioClip> ClipList;
        public float PitchRange = 0.25f;
        public float Volume = 0.2f;
        public bool useCustomPitch;
        public bool useCustomStartPitch;
        public float customStartPitch = 1;
        public bool playLoop;
        
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
