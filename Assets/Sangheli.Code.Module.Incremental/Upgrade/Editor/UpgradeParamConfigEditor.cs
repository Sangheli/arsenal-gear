using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UpgradeParamConfig))]
public class UpgradeParamConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var listProp = serializedObject.FindProperty("Params");
        EditorGUILayout.PropertyField(listProp, true);
        GUILayout.FlexibleSpace(); // растягивает вниз, чтобы заняло всё окно

        serializedObject.ApplyModifiedProperties();
    }
}