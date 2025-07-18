using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataConfigSO", menuName = "Game/Level Data Config")]
public class LevelDataConfigSO : ScriptableObject
{
    public List<LevelConfigItem> Levels = new();
} 