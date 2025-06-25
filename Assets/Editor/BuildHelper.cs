using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class BuildHelper
{
    private static string[] GetEnabledScenes()
    {
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
    }
    
    private static BuildOptions GetBuildOptions()
    {
        return BuildOptions.None;
        return BuildOptions.Development | BuildOptions.AllowDebugging;
    }

    public static void Build(BuildTarget buildTarget, string buildFolder, string exeName = null)
    {
        if (Directory.Exists(buildFolder)) 
            Directory.Delete(buildFolder, true);
        
        Directory.CreateDirectory(buildFolder);
        
        var finalPath = exeName == null ? buildFolder : Path.Combine(buildFolder, exeName);

        BuildPipeline.BuildPlayer
        (
            GetEnabledScenes(),
            finalPath,
            buildTarget,
            GetBuildOptions()
        );
    }
    
    public static string GetName() => 
        Regex.Replace(Application.productName, @"[^a-zA-Z0-9]", "").ToLower();
}