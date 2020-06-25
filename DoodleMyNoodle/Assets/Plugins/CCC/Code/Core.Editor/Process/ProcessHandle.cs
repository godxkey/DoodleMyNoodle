using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEngineX;

public class ProcessHandle : IDisposable
{
    public Action OnExitAction { get; set; }
    public int CustomId { get; private set; }
    public Process Process { get; }
    public bool HasExited => Process.HasExited;

    public ProcessHandle(Process process, int customId = -1)
    {
        this.CustomId = customId;
        this.Process = process ?? throw new NullReferenceException();

        process.Refresh();
        if (process.Id == 0)
            throw new Exception("Process does not appear to be running");

        ProcessHandleManager.RegisterHandle(this);
    }

    /// <summary>
    /// Optional. If the specified process exits, the handle will be disposed of automatically
    /// </summary>
    public void Dispose()
    {
        ProcessHandleManager.UnregisterHandle(this);
    }

    public static ReadOnlyList<ProcessHandle> ActiveHandles
    {
        get
        {
            ProcessHandleManager.InitIfNeeded();
            return ProcessHandleManager.Handles;
        }
    }
}