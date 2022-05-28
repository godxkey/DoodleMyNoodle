using UnityEngine;
using System.Collections.Generic;
using UnityEngineX;
using System;

/// <summary>
/// A GameMonoBehaviour is a monobehaviour class that receives common game events
/// </summary>
public class GameMonoBehaviour : MonoBehaviour, IElementIndexHint
{

    private string GetProfilingMarkerNameForMethod(string methodName) => $"{GetType().Name}.{methodName}";

    private string _profilingMarkerUpdate;
    public string ProfilingMarkerUpdate
    {
        get
        {
            if(_profilingMarkerUpdate == null)
                _profilingMarkerUpdate = GetProfilingMarkerNameForMethod("Update");
            return _profilingMarkerUpdate;
        }
    }

    private string _profilingMarkerFixedUpdate;
    public string ProfilingMarkerFixedUpdate
    {
        get
        {
            if (_profilingMarkerFixedUpdate == null)
                _profilingMarkerFixedUpdate = GetProfilingMarkerNameForMethod("FixedUpdate");
            return _profilingMarkerFixedUpdate;
        }
    }

    private string _profilingMarkerLateUpdate;
    public string ProfilingMarkerLateUpdate
    {
        get
        {
            if (_profilingMarkerLateUpdate == null)
                _profilingMarkerLateUpdate = GetProfilingMarkerNameForMethod("LateUpdate");
            return _profilingMarkerLateUpdate;
        }
    }

    private string _profilingMarkerPresentationUpdate;
    public string ProfilingMarkerPresentationUpdate
    {
        get
        {
            if (_profilingMarkerPresentationUpdate == null)
                _profilingMarkerPresentationUpdate = GetProfilingMarkerNameForMethod("PresentationUpdate");
            return _profilingMarkerPresentationUpdate;
        }
    }

    private string _profilingMarkerPostSimTick;
    public string ProfilingMarkerPostSimTick
    {
        get
        {
            if (_profilingMarkerPostSimTick == null)
                _profilingMarkerPostSimTick = GetProfilingMarkerNameForMethod("PostSimTick");
            return _profilingMarkerPostSimTick;
        }
    }

    private string _profilingMarkerAwake;
    public string ProfilingMarkerAwake
    {
        get
        {
            if (_profilingMarkerAwake == null)
                _profilingMarkerAwake = GetProfilingMarkerNameForMethod("Awake");
            return _profilingMarkerAwake;
        }
    }

    private string _profilingMarkerStart;
    public string ProfilingMarkerStart
    {
        get
        {
            if (_profilingMarkerStart == null)
                _profilingMarkerStart = GetProfilingMarkerNameForMethod("Start");
            return _profilingMarkerStart;
        }
    }

    static NoInterruptList<GameMonoBehaviour> s_registeredBehaviours = new NoInterruptList<GameMonoBehaviour>();

    public static NoInterruptList<GameMonoBehaviour>.Enumerator RegisteredBehaviours => s_registeredBehaviours.GetEnumerator();

    int IElementIndexHint.IndexHint { get; set; }

    protected virtual void Awake()
    {
        s_registeredBehaviours.Add(this);

        if (Game.Ready)
            OnGameAwake();
        
        if (Game.Started && !Game.CallingOnGameStart)
            Game.AddLateStarter(this);
    }

    protected virtual void OnDestroy()
    {
        Game.RemoveLateStarter(this);

        s_registeredBehaviours.Remove(this);
    }

    // callbacks
    public virtual void OnGameUpdate() { }
    public virtual void OnGameFixedUpdate() { }
    public virtual void OnGameLateUpdate() { }
    public virtual void OnGameAwake() { }
    public virtual void OnGameStart() { }
}
