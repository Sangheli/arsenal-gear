using UnityEditor;

public class MacBuilder
{
    [MenuItem("Build/Build Mac")]
    public static void Build()
    {
        var buildFolder = "mac_build";
        BuildHelper.Build(BuildTarget.StandaloneOSX, buildFolder,BuildHelper.GetName());
    }
}