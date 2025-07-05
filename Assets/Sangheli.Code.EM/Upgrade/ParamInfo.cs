using UnityEngine;

public class ParamInfo : BaseUpgradeInfo
{
    public bool UseRandom;
    public float RandomCoef = 1;
    public int boughtActual { get; set; }

    public float GetValueRandom(int bought)
    {
        var currentValue = BaseValue + bought * Step;
        return UseRandom
            ? Random.Range(currentValue, currentValue * RandomCoef)
            : GetValue(bought);
    }
}