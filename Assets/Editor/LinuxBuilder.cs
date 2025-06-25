using UnityEditor;

public class LinuxBuilder
{
    [MenuItem("Build/Build Linux")]
    public static void Build()
    {
        var buildFolder = "linux_build";
        BuildHelper.Build(BuildTarget.StandaloneLinux64, buildFolder,BuildHelper.GetName());
    }
}