using UnityEditor;

public class WebGLBuilder
{
    [MenuItem("Build/Build WebGL")]
    public static void Build()
    {
        var buildFolder = "webgl_build";
        BuildHelper.Build(BuildTarget.WebGL, buildFolder);
    }
}