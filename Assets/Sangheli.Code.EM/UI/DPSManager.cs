public class DPSManager
{
    public static (float,float) GetPlayerDps()
    {
        var saveData = G.I.SaveProvider.Get();
        var paramContainer = saveData.GetParamContainer();
        
        var damage = paramContainer.GetValue("playerDamageMin",onlyDisplay:true);
        var playerFireRate = paramContainer.GetValue("playerFireRate");
        var critMultiplier = paramContainer.GetValue("critMultiplier");

        var averageDamage = damage;
        var averageDamageCrit = damage * critMultiplier;
        var attacksPerSecond = playerFireRate / 60f;
        return (averageDamage * attacksPerSecond, averageDamageCrit * attacksPerSecond);
    }
} 