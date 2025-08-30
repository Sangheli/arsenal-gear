using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Weapon2D[] weapons;
    public Weapon2D currentWeapon;

    public EWeaponType startWeapon = EWeaponType.Laser;
    
    public void Init()
    {
        Select(startWeapon);
    }
    
    public void Select(EWeaponType type)
    {
        foreach (var x in weapons) x.gameObject.SetActive(false);
        var weapon = weapons.FirstOrDefault(x => x.Type == type);
        
        if(weapon==null)
            return;

        currentWeapon = weapon;
        weapon.gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        if(currentWeapon == null)
            return;
        
        currentWeapon.SetPos(G.I.playerCursor.transform.position);
    }

    public void Shoot()
    {
        if(currentWeapon == null)
            return;
        
        currentWeapon.Shoot();
    }
}