using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class CoreServiceManager
{
    class ServiceInitJoin : AsyncOperationJoin<ICoreService>
    {
        CoreServiceManager _owner;
        public ServiceInitJoin(CoreServiceManager owner, Action onJoin) : base(onJoin)
        {
            this._owner = owner;
        }

        protected override void OnCompleteAnyInit(ICoreService obj)
        {
            _owner.OnAnyServiceInitComplete(obj);
            base.OnCompleteAnyInit(obj);
        }
    }

    class InitEvent
    {
        public SafeEvent Callbacks = new SafeEvent();
        public bool CallbacksHaveBeenSent = false;
    }

    Dictionary<Type, ICoreService> _services = new Dictionary<Type, ICoreService>();

    static public bool InitializationComplete { get; private set; } = false;
    static public CoreServiceManager Instance { get; private set; }
    static InitEvent s_onAllInitComplete = new InitEvent();
    static Dictionary<Type, InitEvent> s_initializationCallbackMap = new Dictionary<Type, InitEvent>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReset()
    {
        s_onAllInitComplete = new InitEvent();
        s_initializationCallbackMap = new Dictionary<Type, InitEvent>();
        InitializationComplete = false;
        Instance = null;
    }


    static public void AddInitializationCallback<T>(Action onComplete) where T : ICoreService
    {
        if (onComplete == null)
        {
            Log.Error("CoreServiceManager.AddInitializationCallback: Received a null callback.");
            return;
        }

        if (InitializationComplete)
        {
            onComplete();
        }
        else
        {
            InitEvent initEvent;
            s_initializationCallbackMap.TryGetValue(typeof(T), out initEvent);

            if (initEvent == null) // The even is null ? Create a new one for type 'T'
            {
                initEvent = new InitEvent();
                s_initializationCallbackMap.Add(typeof(T), initEvent);
            }

            if (initEvent.CallbacksHaveBeenSent) // if callbacks have already been sent, dont add it to the list
            {
                onComplete();
            }
            else
            {
                initEvent.Callbacks += onComplete;
            }
        }
    }
    static public void RemoveInitializationCallback<T>(Action onComplete) where T : ICoreService
    {
        if (InitializationComplete == false)
        {
            InitEvent safeEvent;
            if (s_initializationCallbackMap.TryGetValue(typeof(T), out safeEvent))
            {
                if (safeEvent.CallbacksHaveBeenSent == false) // there's no point in removing the callback if they've already been sent
                {
                    safeEvent.Callbacks.RemoveAction(onComplete);
                }
            }
        }
    }

    static public void AddInitializationCallback(Action onComplete)
    {
        if (onComplete == null)
        {
            Log.Error("CoreServiceManager.AddInitializationCallback: Received a null callback.");
            return;
        }

        if (InitializationComplete)
        {
            onComplete();
        }
        else
        {
            s_onAllInitComplete.Callbacks += onComplete;
        }
    }
    static public void RemoveInitializationCallback(Action onComplete)
    {
        if (s_onAllInitComplete != null && s_onAllInitComplete.CallbacksHaveBeenSent == false)
        {
            s_onAllInitComplete.Callbacks -= onComplete;
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
            _services.Add(officialInstance.GetType(), officialInstance);
        }

        // Initialize them all
        ServiceInitJoin join = new ServiceInitJoin(this, OnInitializationComplete);
        foreach (KeyValuePair<Type, ICoreService> servicePair in _services)
        {
            servicePair.Value.Initialize(join.RegisterOperation());
        }
        join.MarkEnd();
    }

    public T GetCoreService<T>() where T : ICoreService
    {
        return (T)_services[typeof(T)];
    }

    void OnAnyServiceInitComplete(ICoreService service)
    {
        // Execute the 'on init complete' event related to THAT service
        InitEvent initEvent;
        if (s_initializationCallbackMap.TryGetValue(service.GetType(), out initEvent))
        {
            initEvent.Callbacks.SafeInvoke();
            initEvent.CallbacksHaveBeenSent = true;
        }
    }

    void OnInitializationComplete()
    {
        InitializationComplete = true;

        s_onAllInitComplete.Callbacks.SafeInvoke();
        s_onAllInitComplete.CallbacksHaveBeenSent = true;

        //clean up
        s_onAllInitComplete = null;
        s_initializationCallbackMap = null;
    }
}
