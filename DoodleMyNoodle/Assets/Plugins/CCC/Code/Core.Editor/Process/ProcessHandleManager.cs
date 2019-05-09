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

    static void Save()
    {
        for (int i = 0; i < handles.Count; i++)
        {
            EditorPrefs.SetInt("ProcessHandle-" + i, handles[i].process.Id);
        }
        EditorPrefs.SetInt("ProcessHandle-" + handles.Count, 0);
    }

    static List<int> loadList = new List<int>();
    static void Load()
    {
        loadList.Clear();

        int i = 0;
        while (true)
        {
            int savedId = EditorPrefs.GetInt("ProcessHandle-" + i, 0);

            if (savedId == 0)
                break;

            loadList.Add(savedId);
            i++;
        }


        Process[] runningProcesses = Process.GetProcesses();

        foreach (int processId in loadList)
        {
            Process process = runningProcesses.Find((x) => x.Id == processId);

            if (process != null)
            {
                new ProcessHandle(process);
            }
        }

        Save();
    }


    static bool hasInit = false;
    static Action onInit;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnRuntimeMethodLoad()
    {
        if (hasInit)
            return;
        hasInit = true;
        Load();

        onInit?.Invoke();
    }

    public static void RegisterOnInitCallback(Action callback)
    {
        if (hasInit)
            callback();
        else
            onInit += callback;
    }
}