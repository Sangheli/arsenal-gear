using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public void OnStartRound()
    {
        var _save = G.I.SaveProvider.Get();
        var paramContainer = _save.GetParamContainer();
        var mCount = (int) paramContainer.GetValue("u_machineGunCount");
        var lCount = (int) paramContainer.GetValue("u_LaserCount");
        
        Select(
                    new List<EWeaponType>
                    {
                    EWeaponType.MachineGun,
                    EWeaponType.Laser
                }, 
                    new List<int>
                    {
                    mCount,
                    lCount
                }
            );
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
    
    private void LateUpdateNew()
    {
        if(currentWeapons == null)
            return;
        
        if (currentWeapons.Count == 0)
            return;
        
        var screenPos = GetScreenPos();

        foreach (var w in currentWeapons)
            w.SetPos(screenPos);
    }

    private static Vector2 GetScreenPos()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();
        
        if(Mouse.current != null)
            return Mouse.current.position.ReadValue();
        
        return Vector2.zero;
    }

    public void Shoot(Vector3 endPos)
    {
        if (currentWeapons.Count == 0)
            return;
        
        var uniqueTypes = currentWeapons.Select(w => w.Type).Distinct();
        foreach (var type in uniqueTypes)
            PlaySound(type);

        
        if (currentWeapons.Count < 2 + 1)
        {
            ShootAll(currentWeapons,endPos);
        }
        else
        {
            var randomBounce = currentWeapons.PickRandom(Random.Range(2, currentWeapons.Count + 1));
            ShootAll(randomBounce,endPos);    
        }
    }

    private static void ShootAll(List<Weapon2D> randomBounce,Vector3 endPos)
    {
        foreach (var w in randomBounce)
            w.Shoot(endPos);
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