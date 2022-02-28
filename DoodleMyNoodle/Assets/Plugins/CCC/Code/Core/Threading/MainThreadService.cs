using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngineX;

public class MainThreadService : MonoCoreService<MainThreadService>
{
    private static Queue<Action> s_mainThreadCallbacks = new Queue<Action>();

    static public int MainThreadId
    {
        get
        {
            if (s_mainThreadIdSet == false)
            {
                Debug.LogError("The main thread Id has not been set yet. Please create an instance of " + nameof(MainThreadService) + " first.");
            }
            return s_mainThreadId;
        }
        set
        {
            s_mainThreadId = value;
            s_mainThreadIdSet = true;
        }
    }

    static private bool s_mainThreadIdSet = false;
    static private int s_mainThreadId;

    public override void Initialize(Action<ICoreService> onComplete)
    {
        s_mainThreadCallbacks.Clear();
        MainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        onComplete(this);
    }

    void Update()
    {
        while (s_mainThreadCallbacks.Count > 0)
        {
            s_mainThreadCallbacks.Dequeue().InvokeCatchExceptionInEditor();
        }
    }

    static public void AddMainThreadCallbackFromThread(Action action)
    {
        if (action == null)
            return;

        if (Instance == null)
        {
            Debug.LogError("No instance of " + nameof(MainThreadService) + " has been found. Please add it to the list of CoreServices in the Resources folder.");
            return;
        }

        lock (Instance)
        {
            s_mainThreadCallbacks.Enqueue(action);
        }
    }
}
