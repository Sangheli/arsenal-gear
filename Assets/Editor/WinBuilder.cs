using UnityEditor;

public class WinBuilder
{
    [MenuItem("Build/Build Win")]
    public static void Build()
    {
        var buildFolder = "pc_build";
        BuildHelper.Build(BuildTarget.StandaloneWindows64, buildFolder,$"{BuildHelper.GetName()}.exe");
    }
}