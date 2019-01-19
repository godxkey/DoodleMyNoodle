using UnityEngine;
using System.Collections.Generic;
using System;

public class MainThreadService : MonoCoreService<MainThreadService>
{
    private static Queue<Action> mainThreadCallbacks = new Queue<Action>();

    static public int MainThreadId
    {
        get
        {
            if (mainThreadIdSet == false)
            {
                Debug.LogError("The main thread Id has not been set yet. Please create an instance of " + nameof(MainThreadService) + " first.");
            }
            return mainThreadId;
        }
        set
        {
            mainThreadId = value;
            mainThreadIdSet = true;
        }
    }

    static private bool mainThreadIdSet = false;
    static private int mainThreadId;

    public override void Initialize(Action<ICoreService> onComplete)
    {
        MainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        onComplete(this);
    }

    void Update()
    {
        while (mainThreadCallbacks.Count > 0)
        {
            mainThreadCallbacks.Dequeue().SafeInvokeInEditor();
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
            mainThreadCallbacks.Enqueue(action);
        }
    }
}
