using UnityEngine;
using System.Reflection;

[CreateAssetMenu(menuName = "Sangheli/DataContainer")]
public class DataContainer : ScriptableObject
{
    public SoundContainer soundContainer;
    public SpriteContainer spriteContainer;
    public FontContainer fontContainer;
    public LevelDataConfigSO levelDataConfigSO; // ScriptableObject с уровнями
    public UpgradeParamConfig upgradeParamConfig; // ScriptableObject с параметрами апгрейдов
    
    public void Init()
    {
        var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        
        foreach (var field in fields)
        {
            // Пропускаем, если поле уже установлено
            if (field.GetValue(this) != null) continue;

            var fieldType = field.FieldType;

            // Загружаем ресурс по типу
            var assets = Resources.LoadAll("", fieldType);
            if (assets.Length > 0) field.SetValue(this, assets[0]); // берем первый найденный
        }
    }
}