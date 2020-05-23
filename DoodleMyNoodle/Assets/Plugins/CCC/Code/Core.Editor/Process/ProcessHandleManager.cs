using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEditor;
using UnityEngineX;

internal class ProcessHandleManager
{
    static List<ProcessHandle> _handles = new List<ProcessHandle>();
    public static ReadOnlyList<ProcessHandle> Handles => _handles.AsReadOnlyNoAlloc();

    static bool _updateRegistered = false;
    static int _updateIterator;

    public static void RegisterHandle(ProcessHandle handle)
    {
        _handles.Add(handle);

        if (_updateRegistered == false)
        {
            EditorApplication.update += EditorUpdate;
            _updateRegistered = true;
        }

        Save();
    }

    public static void UnregisterHandle(ProcessHandle handle)
    {
        _handles.RemoveWithLastSwap(handle);
        _updateIterator--;

        if (_handles.Count == 0 && _updateRegistered)
        {
            EditorApplication.update -= EditorUpdate;
            _updateRegistered = false;
        }

        Save();
    }

    static void EditorUpdate()
    {
        for (_updateIterator = 0; _updateIterator < _handles.Count; _updateIterator++)
        {
            ProcessHandle handle = _handles[_updateIterator];

            if (handle.HasExited)
            {
                handle.OnExitAction?.Invoke();
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
        EditorPrefs.SetInt("ProcessHandleCount", _handles.Count);
        for (int i = 0; i < _handles.Count; i++)
        {
            EditorPrefs.SetInt($"ProcessHandleProcessId-{i}", _handles[i].Process.Id);
            EditorPrefs.SetInt($"ProcessHandleCustomId-{i}", _handles[i].CustomId);
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