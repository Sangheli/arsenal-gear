using UnityEngine;

public class ParamInfo : BaseUpgradeInfo
{
    public bool UseRandom;
    public float RandomCoef = 1;

    public override float GetValue(int bought, bool onlyDisplay = false)
    {
        var currentValue = BaseValue + bought * Step;
        return UseRandom && !onlyDisplay
            ? Random.Range(currentValue, currentValue * RandomCoef)
            : Mathf.Max(currentValue, BaseValue);
    }
}