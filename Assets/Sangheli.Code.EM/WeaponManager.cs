using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Weapon2D[] weapons;
    public List<Weapon2D> currentWeapons = new(); // теперь список

    public List<EWeaponType> startWeaponList = new()
    {
        EWeaponType.MachineGun,
    };

    public List<int> startCount = new()
    {
        2
    };

    public void Init()
    {
        Select(startWeaponList, startCount);
    }
    
    public void Select(List<EWeaponType> typeList, List<int> count)
    {
        foreach (var x in weapons) 
            x.gameObject.SetActive(false);

        currentWeapons.Clear();

        for (int i = 0; i < typeList.Count; i++)
        {
            var type = typeList[i];
            var needCount = count[i];

            var sameTypeWeapons = weapons.Where(x => x.Type == type).Take(needCount);

            foreach (var w in sameTypeWeapons)
            {
                w.gameObject.SetActive(true);
                currentWeapons.Add(w);
            }
        }
    }

    private void LateUpdate()
    {
        if (currentWeapons.Count == 0)
            return;
        
        foreach (var w in currentWeapons)
            w.SetPos(G.I.playerCursor.transform.position);
    }

    public void Shoot()
    {
        if (currentWeapons.Count == 0)
            return;
        
        var uniqueTypes = currentWeapons.Select(w => w.Type).Distinct();
        foreach (var type in uniqueTypes)
            PlaySound(type);

        
        if (currentWeapons.Count < 2 + 1)
        {
            ShootAll(currentWeapons);
        }
        else
        {
            var randomBounce = currentWeapons.PickRandom(Random.Range(2, currentWeapons.Count + 1));
            ShootAll(randomBounce);    
        }
    }

    private static void ShootAll(List<Weapon2D> randomBounce)
    {
        foreach (var w in randomBounce)
            w.Shoot();
    }
    
    private void PlaySound(EWeaponType Type)
    {
        switch (Type)
        {
            case EWeaponType.None:
                break;
            case EWeaponType.MachineGun:
                G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.hit);
                break;
            case EWeaponType.Laser:
                G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.laserShoot);
                break;
        }
    }
}