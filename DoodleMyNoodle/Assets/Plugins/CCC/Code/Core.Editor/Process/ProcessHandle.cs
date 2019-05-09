using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

public class ProcessHandle : IDisposable
{
    public Action onExitAction { get; set; }
    public int id { get; set; }
    public Process process { get; }
    public bool hasExited => process.HasExited;

    public ProcessHandle(Process process)
    {
        this.process = process ?? throw new NullReferenceException();

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

    public static ReadOnlyCollection<ProcessHandle> activeHandles => ProcessHandleManager.handlesReadOnly;
    public static void RegisterOnInitCallback(Action callback) => ProcessHandleManager.RegisterOnInitCallback(callback);
}