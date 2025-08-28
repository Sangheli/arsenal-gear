using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyData))]
public class EnemyDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnemyData data = (EnemyData)target;

        GUILayout.Space(10);
        GUILayout.Label("Sprites Preview", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        if (data.spriteView != null)
            DrawSpritePreview(data.spriteView, "View");

        if (data.spriteBorder != null)
            DrawSpritePreview(data.spriteBorder, "Border");

        GUILayout.EndHorizontal();
        
        // Отрисовываем стандартные поля
        DrawDefaultInspector();
    }

    private void DrawSpritePreview(Sprite sprite, string label)
    {
        GUILayout.BeginVertical(GUILayout.Width(120));

        Rect rect = GUILayoutUtility.GetRect(120, 120, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

        Texture2D tex = sprite.texture;
        Rect texRect = sprite.textureRect;
        Rect uv = new Rect(
            texRect.x / tex.width,
            texRect.y / tex.height,
            texRect.width / tex.width,
            texRect.height / tex.height);

        GUI.DrawTextureWithTexCoords(rect, tex, uv, true);

        GUILayout.Label(label, EditorStyles.centeredGreyMiniLabel);

        GUILayout.EndVertical();
    }
}