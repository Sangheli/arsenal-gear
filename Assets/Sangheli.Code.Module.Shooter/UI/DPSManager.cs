using System.Linq;

public class DPSManager
{
    public static string[] keys = 
    {
        "playerDamageMin",
        "playerDamageMin2",
        "playerDamageMin3",
        "playerDamageMin4",
        "playerDamageMin5",
    };
    
    public static string[] keysWeapon = 
    {
        "u_machineGunCount",
        "u_LaserCount",
    };
    
    public static string[] keysCrit = 
    {
        "critMultiplier",
    };

    public static float GetPlayerDpsMin()
    {
        var saveData = G.I.SaveProvider.Get();
        var paramContainer = saveData.GetParamContainer();
        return paramContainer.GetBaseValue(keys[0]);
    }
    
    public static (float,float) GetCurrentPlayerDps()
    {
        var saveData = G.I.SaveProvider.Get();
        var paramContainer = saveData.GetParamContainer();
        
        var damage = keys.Sum(x => paramContainer.GetValue(x));
        var superMult = GetCurrentSuperMult();
        damage *= superMult;
        var playerFireRate = FireRateManager.GetPlayerFireRate();
        var critMultiplier = paramContainer.GetValue(keysCrit[0]);

        var averageDamage = damage;
        var averageDamageCrit = damage * critMultiplier;
        var attacksPerSecond = playerFireRate / 60f;
        return (averageDamage * attacksPerSecond, averageDamageCrit * attacksPerSecond);
    }
    
    public static (float,float) GetDpsMax()
    {
        var damage = G.I.Data.ParamData.Params.Where(x => keys.Contains(x.Key)).Sum(x => x.MaxValue);
        var superMult = G.I.Data.ParamData.Params.Where(x=>keysWeapon.Contains(x.Key)).Sum(x => x.MaxValue);
        var playerFireRate = G.I.Data.ParamData.Params.Where(x=>FireRateManager.keys.Contains(x.Key)).Sum(x => x.MaxValue);
        var critMultiplier = G.I.Data.ParamData.Params.Where(x=>keysCrit.Contains(x.Key)).Sum(x => x.MaxValue);
        
        damage *= superMult;
        var averageDamage = damage;
        var averageDamageCrit = damage * critMultiplier;
        var attacksPerSecond = playerFireRate / 60f;
        return (averageDamage * attacksPerSecond, averageDamageCrit * attacksPerSecond);
    }

    private static int GetCurrentSuperMult()
    {
        var saveData = G.I.SaveProvider.Get();
        var paramContainer = saveData.GetParamContainer();
        var mCount = (int) paramContainer.GetValue("u_machineGunCount");
        var lCount = (int) paramContainer.GetValue("u_LaserCount");
        return mCount + lCount;
    }
} 