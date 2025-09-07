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

    public static float GetPlayerDpsMin()
    {
        var saveData = G.I.SaveProvider.Get();
        var paramContainer = saveData.GetParamContainer();
        return paramContainer.GetBaseValue(keys[0]);
    }
    
    public static (float,float) GetPlayerDps()
    {
        var saveData = G.I.SaveProvider.Get();
        var paramContainer = saveData.GetParamContainer();
        
        var damage = keys.Sum(x => paramContainer.GetValue(x));
        var superMult = GetSuperMult();
        damage *= superMult;
        var playerFireRate = paramContainer.GetValue("playerFireRate");
        var critMultiplier = paramContainer.GetValue("critMultiplier");

        var averageDamage = damage;
        var averageDamageCrit = damage * critMultiplier;
        var attacksPerSecond = playerFireRate / 60f;
        return (averageDamage * attacksPerSecond, averageDamageCrit * attacksPerSecond);
    }
    
    
    private static int GetSuperMult()
    {
        var saveData = G.I.SaveProvider.Get();
        var paramContainer = saveData.GetParamContainer();
        var mCount = (int) paramContainer.GetValue("u_machineGunCount");
        var lCount = (int) paramContainer.GetValue("u_LaserCount");
        return mCount + lCount;
    }
} 