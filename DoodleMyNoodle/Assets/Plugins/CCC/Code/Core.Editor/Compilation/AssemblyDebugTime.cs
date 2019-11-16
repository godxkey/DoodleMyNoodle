using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

[InitializeOnLoad]
public class AsmdefDebug
{//
    const string ASSEMBLY_RELOAD_EVENTS_EDITOR_PREF = "AssemblyReloadEventsTime";
    const string ASSEMBLY_COMPILATION_EVENTS_EDITOR_PREF = "AssemblyCompilationEvents";
    static readonly int s_scriptAssembliesPathLen = "Library/ScriptAssemblies/".Length;

    static Dictionary<string, DateTime> s_startTimes = new Dictionary<string, DateTime>();

    static StringBuilder s_buildEvents = new StringBuilder();
    static double s_compilationTotalTime;

    static AsmdefDebug()
    {
        CompilationPipeline.assemblyCompilationStarted += CompilationPipelineOnAssemblyCompilationStarted;
        CompilationPipeline.assemblyCompilationFinished += CompilationPipelineOnAssemblyCompilationFinished;
        AssemblyReloadEvents.beforeAssemblyReload += AssemblyReloadEventsOnBeforeAssemblyReload;
        AssemblyReloadEvents.afterAssemblyReload += AssemblyReloadEventsOnAfterAssemblyReload;
    }

    static void CompilationPipelineOnAssemblyCompilationStarted(string assembly)
    {
        s_startTimes[assembly] = DateTime.UtcNow;
    }

    static void CompilationPipelineOnAssemblyCompilationFinished(string assembly, CompilerMessage[] arg2)
    {
        TimeSpan timeSpan = DateTime.UtcNow - s_startTimes[assembly];
        s_compilationTotalTime += timeSpan.TotalMilliseconds;
        s_buildEvents.AppendFormat("{0:0.00}s {1}\n", timeSpan.TotalMilliseconds / 1000f, assembly.Substring(s_scriptAssembliesPathLen, assembly.Length - s_scriptAssembliesPathLen));
    }

    static void AssemblyReloadEventsOnBeforeAssemblyReload()
    {
        s_buildEvents.AppendFormat("compilation total: {0:0.00}s\n", s_compilationTotalTime / 1000f);
        EditorPrefs.SetString(ASSEMBLY_RELOAD_EVENTS_EDITOR_PREF, DateTime.UtcNow.ToBinary().ToString());
        EditorPrefs.SetString(ASSEMBLY_COMPILATION_EVENTS_EDITOR_PREF, s_buildEvents.ToString());
    }

    static void AssemblyReloadEventsOnAfterAssemblyReload()
    {
        string binString = EditorPrefs.GetString(ASSEMBLY_RELOAD_EVENTS_EDITOR_PREF);

        if (long.TryParse(binString, out long bin))
        {
            DateTime date = DateTime.FromBinary(bin);
            TimeSpan time = DateTime.UtcNow - date;
            string compilationTimes = EditorPrefs.GetString(ASSEMBLY_COMPILATION_EVENTS_EDITOR_PREF);
            if (!string.IsNullOrEmpty(compilationTimes))
            {
                // UNCOMMENT TO GET COMPILATION REPORTS
                // Debug.Log("Compilation Report\n" + compilationTimes + "Assembly Reload Time: " + time.TotalSeconds + "s\n");
            }
        }
    }
}
