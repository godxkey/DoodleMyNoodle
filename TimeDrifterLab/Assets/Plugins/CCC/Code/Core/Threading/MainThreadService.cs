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
            if(mainThreadIdSet == false)
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

    void Awake()
    {
        MainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
    }

    void Update()
    {
        while (mainThreadCallbacks.Count > 0)
        {
            mainThreadCallbacks.Dequeue().Invoke();
        }
    }

    static public void AddCallbackFromThread(Action action)
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
