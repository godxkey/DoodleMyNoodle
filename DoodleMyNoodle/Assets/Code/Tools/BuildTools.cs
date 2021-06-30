using System;
using System.IO;
using Unity.Build;
using UnityEditor;
using UnityEngine;
using UnityEngineX;

public class BuildTools
{
    public static void BuildGame(bool andRun)
    {
        KillProcessesAndStopEditor();

        BuildConfiguration buildConfig = GetBuildConfig();
        BuildResult buildResult = buildConfig.Build();

        buildResult.LogResult();

        if (andRun && buildResult.Succeeded)
        {
            RunResult runResult = buildConfig.Run();
            runResult.LogResult();
        }
    }

    public static void KillProcessesAndStopEditor()
    {
        var lastBuildFile = GetLastBuildFile();

        if (lastBuildFile != null)
        {
            KillAllProcesses(lastBuildFile.Name);
        }

        EditorApplication.isPlaying = false;
    }

    public static FileInfo GetLastBuildFile()
    {
        Type windowsArtifactType = TypeUtility.FindType("Unity.Build.Windows.Classic.WindowsArtifact", throwOnError: true);
        var fileInfoField = windowsArtifactType.GetField("OutputTargetFile");
        foreach (var artifact in GetBuildConfig().GetAllBuildArtifacts())
        {
            if (artifact.GetType() == windowsArtifactType)
            {
                FileInfo fileInfo = fileInfoField.GetValue(artifact) as FileInfo;
                if (fileInfo != null)
                {
                    return fileInfo;
                }
            }
        }
        return null;
    }


    public static void KillAllProcesses(string execName)
    {
        string processName = Path.GetFileNameWithoutExtension(execName);

        System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
        foreach (var process in processes)
        {
            if (process.HasExited)
                continue;

            try
            {
                if (process.ProcessName != null && process.ProcessName == processName)
                {
                    process.Kill();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error when trying to kill all processes: " + e.ToString());
            }
        }
    }

    public static BuildConfiguration GetBuildConfig()
    {
        return AssetDatabase.LoadAssetAtPath<BuildConfiguration>(PipelineSettings.WindowsBuildConfiguration);
    }
}

// Convenience functions for strings
public static class StringExtensionMethods
{
    public static string AfterLast(this string str, string sub)
    {
        var idx = str.LastIndexOf(sub);
        return idx < 0 ? "" : str.Substring(idx + sub.Length);
    }

    public static string BeforeLast(this string str, string sub)
    {
        var idx = str.LastIndexOf(sub);
        return idx < 0 ? "" : str.Substring(0, idx);
    }

    public static string AfterFirst(this string str, string sub)
    {
        var idx = str.IndexOf(sub);
        return idx < 0 ? "" : str.Substring(idx + sub.Length);
    }

    public static string BeforeFirst(this string str, string sub)
    {
        var idx = str.IndexOf(sub);
        return idx < 0 ? "" : str.Substring(0, idx);
    }
}