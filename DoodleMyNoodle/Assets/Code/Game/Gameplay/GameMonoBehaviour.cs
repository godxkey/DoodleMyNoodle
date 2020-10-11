using UnityEngine;
using System.Collections.Generic;
using UnityEngineX;
using System;

/// <summary>
/// A GameMonoBehaviour is a monobehaviour class that receives common game events
/// </summary>
public class GameMonoBehaviour : MonoBehaviour, IElementIndexHint
{
    static NoInterruptList<GameMonoBehaviour> s_registeredBehaviours = new NoInterruptList<GameMonoBehaviour>();
    public static NoInterruptList<GameMonoBehaviour>.Enumerator RegisteredBehaviours => s_registeredBehaviours.GetEnumerator();

    int IElementIndexHint.IndexHint { get; set; }

    protected virtual void Awake()
    {
        s_registeredBehaviours.Add(this);

        if (Game.Ready)
            OnGameAwake();
        
        if (Game.Started)
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
