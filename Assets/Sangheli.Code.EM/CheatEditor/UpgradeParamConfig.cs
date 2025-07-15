using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeParamConfig", menuName = "Configs/UpgradeParamConfig")]
public class UpgradeParamConfig : ScriptableObject
{
    public List<ParamInfo> Params = new();
    public ParamInfo GetParamInfo(string key) => Params.FirstOrDefault(x => x.Key == key);
}