using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class AndroidBuilder
{
    [MenuItem("Build/Build Android")]
    public static void Build()
    {
        Debug.Log($"Build Start");

        BuildHelper.Build(BuildTarget.Android, "androidBuild", $"{BuildHelper.GetName()}.apk");

        Debug.Log($"Build Done");
    }
}
