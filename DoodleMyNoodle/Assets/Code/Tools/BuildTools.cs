using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildTools
{
    public static BuildReport BuildGame(
        string buildPath,
        string exeName,
        BuildOptions options,
        string buildId,
        string[] scenes,
        BuildTarget buildTarget/*, 
        string[] scriptingSymbols*/)
    {
        switch (buildTarget)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                break;
            default:
                Debug.LogError("Unsupported build target (for the moment)");
                return null;
        }


        string exePathName = buildPath + "/" + exeName;

        Debug.Log("Building: " + exePathName);
        Directory.CreateDirectory(buildPath);

        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
        
        //string previousSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", scriptingSymbols));

        Environment.SetEnvironmentVariable("BUILD_ID", buildId, EnvironmentVariableTarget.Process);
        BuildReport report = BuildPipeline.BuildPlayer(scenes, exePathName, buildTarget, options);
        Environment.SetEnvironmentVariable("BUILD_ID", "", EnvironmentVariableTarget.Process);

        int stepCount = report.steps.Length;
        Debug.Log(" Steps:" + stepCount);
        for (var i = 0; i < stepCount; i++)
        {
            var step = report.steps[i];
            Debug.Log("-- " + (i + 1) + "/" + stepCount + " " + step.name + " " + step.duration.Seconds + "s --");
            foreach (var msg in step.messages)
                Debug.Log(msg.content);
        }

        Debug.Log($"<color=green> ==== Build Done ({report.summary.totalTime.TotalSeconds}s) ===== </color>");
        
        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, previousSymbols);

        return report;
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