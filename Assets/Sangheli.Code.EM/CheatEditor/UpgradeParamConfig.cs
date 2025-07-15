using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeParamConfig", menuName = "Configs/UpgradeParamConfig")]
public class UpgradeParamConfig : ScriptableObject
{
    public List<UpgradeParamEntry> Upgrades = new();
}