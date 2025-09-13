using System.Linq;

public class FireRateManager
{
    public static string[] keys = 
    {
        "playerFireRate",
        "playerFireRate2",
        "playerFireRate3",
        "playerFireRate4",
        "playerFireRate5",
    };
    
    public static float GetPlayerFireRate()
    {
        var saveData = G.I.SaveProvider.Get();
        var paramContainer = saveData.GetParamContainer();
        return keys.Sum(x => paramContainer.GetValue(x));
    }
}