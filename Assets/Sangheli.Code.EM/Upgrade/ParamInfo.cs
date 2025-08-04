using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[Serializable]
public class ParamInfo : BaseUpgradeInfo
{
    public string Key;
    public bool UseRandom;
    public float RandomCoef = 1;
    public int boughtActual { get; set; }
    
    /// <summary>
    /// List of keys that must be fully purchased before this upgrade becomes available
    /// </summary>
    public List<string> fullDepentList = new List<string>();
    
    /// <summary>
    /// List of keys where at least one level must be purchased from each key before this upgrade becomes available
    /// </summary>
    public List<string> anyDepnetList = new List<string>();

    public float GetValueRandom(int bought)
    {
        var currentValue = BaseValue + bought * Step;
        return UseRandom
            ? Random.Range(currentValue, currentValue * RandomCoef)
            : GetValue(bought);
    }
    
    public float GetValueRandomFromValue(float value) => Random.Range(value, value * RandomCoef);
}