using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreServiceManager
{
    Dictionary<Type, ICoreService> services = new Dictionary<Type, ICoreService>();

    static bool initializationComplete = false;
    static List<Action> initializationCallbacks = new List<Action>();
    static public CoreServiceManager Instance { get; private set; }

    static public void AddInitializationCallback(Action onComplete)
    {
        if (onComplete == null)
        {
            Debug.LogError("CoreServiceManager.AddInitializationCallback: Received a null callback.");
        }

        if (initializationComplete)
        {
            onComplete();
        }
        else
        {
            initializationCallbacks.Add(onComplete);
        }
    }

    public CoreServiceManager()
    {
        Debug.Assert(Instance == null);
        Instance = this;

        CoreServiceBank bank = CoreServiceBank.LoadBank();

        // Spawn core services
        List<ICoreService> unofficialServiceList = bank.GetCoreServices();
        foreach (ICoreService service in unofficialServiceList)
        {
            ICoreService officialInstance = service.ProvideOfficialInstance();
            services.Add(officialInstance.GetType(), officialInstance);
        }

        // Initialize them all
        AsyncOperationJoin join = new AsyncOperationJoin(OnInitializationComplete);
        foreach (KeyValuePair<Type, ICoreService> servicePair in services)
        {
            servicePair.Value.Initialize(join.RegisterOperation());
        }
        join.MarkEnd();
    }

    public T GetCoreService<T>() where T : ICoreService
    {
        return (T)services[typeof(T)];
    }


    void OnInitializationComplete()
    {
        initializationComplete = true;

        foreach (Action callback in initializationCallbacks)
        {
            callback?.SafeInvoke();
        }

        initializationCallbacks.Clear();
    }
}
