using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelObjectiveVisualConfig", menuName = "ScriptableObjects/LevelObjectiveVisualConfig")]
public class LevelObjectiveVisualConfig : ScriptableObject
{
    [System.Serializable]
    public class ObjectiveVisualData
    {
        public Color color = Color.white;
        public LevelObjectiveType objectiveType;
        public Sprite sprite;
    }

    public List<ObjectiveVisualData> objectiveVisuals = new();

    public ObjectiveVisualData Get(LevelObjectiveType objectiveType) => 
        objectiveVisuals.Find(v => v.objectiveType == objectiveType);
}