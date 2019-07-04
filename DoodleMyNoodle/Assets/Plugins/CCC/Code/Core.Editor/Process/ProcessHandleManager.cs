using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

internal class ProcessHandleManager
{
    static List<ProcessHandle> handles = new List<ProcessHandle>();
    public static ReadOnlyCollection<ProcessHandle> handlesReadOnly = handles.AsReadOnly();
    static bool updateRegistered = false;

    static int updateIterator;

    public static void RegisterHandle(ProcessHandle handle)
    {
        handles.Add(handle);

        if (updateRegistered == false)
        {
            EditorApplication.update += EditorUpdate;
            updateRegistered = true;
        }

        Save();
    }

    public static void UnregisterHandle(ProcessHandle handle)
    {
        handles.RemoveWithLastSwap(handle);
        updateIterator--;

        if (handles.Count == 0 && updateRegistered)
        {
            EditorApplication.update -= EditorUpdate;
            updateRegistered = false;
        }

        Save();
    }

    static void EditorUpdate()
    {
        for (updateIterator = 0; updateIterator < handles.Count; updateIterator++)
        {
            ProcessHandle handle = handles[updateIterator];

            if (handle.hasExited)
            {
                handle.onExitAction?.Invoke();
                UnregisterHandle(handle);
            }
        }
    }
    /*
     * Save schema:
     * 
     * -- ProcessHandleCount {process count} --
     * ProcessHandleCount 4
     * 
     * -- ProcessHandleProcessId-{i} {windows process id} --
     * ProcessHandleProcessId-0 86119
     * ProcessHandleProcessId-1 34887
     * ProcessHandleProcessId-2 67870
     * ProcessHandleProcessId-3 12844
     * 
     * -- ProcessHandleCustomId-{i} {the id our editor system attributed to it} --
     * ProcessHandleCustomId-0 12
     * ProcessHandleCustomId-1 2
     * ProcessHandleCustomId-2 3
     * ProcessHandleCustomId-3 5
     * 
     * */

    static void Save()
    {
        EditorPrefs.SetInt("ProcessHandleCount", handles.Count);
        for (int i = 0; i < handles.Count; i++)
        {
            EditorPrefs.SetInt($"ProcessHandleProcessId-{i}", handles[i].process.Id);
            EditorPrefs.SetInt($"ProcessHandleCustomId-{i}", handles[i].customId);
            //UnityEngine.Debug.Log($"Saved:  ProcessHandleProcessId-{i}: {handles[i].process.Id} |   ProcessHandleCustomId-{i}: {handles[i].customId}");
        }
    }

    static void Load()
    {
        int processHandleCount = EditorPrefs.GetInt("ProcessHandleCount", defaultValue: 0);

        Process[] runningProcesses = Process.GetProcesses();
        for (int i = 0; i < processHandleCount; i++)
        {
            int processId = EditorPrefs.GetInt($"ProcessHandleProcessId-{i}");
            int customId = EditorPrefs.GetInt($"ProcessHandleCustomId-{i}");
            //UnityEngine.Debug.Log($"Loaded:  ProcessHandleProcessId-{i}: {processId} |   ProcessHandleCustomId-{i}: {customId}");
            Process process = runningProcesses.Find((x) => x.Id == processId);

            if (process != null)
            {
                ProcessHandle processHandle = new ProcessHandle(process, customId);
            }
        }

        Save();
    }


    static bool hasInit = false;
    public static void InitIfNeeded()
    {
        if (hasInit)
            return;
        hasInit = true;
        Load();
    }
}