using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A GameMonoBehaviour is a monobehaviour class that receives common game events
/// </summary>
public class GameMonoBehaviour : MonoBehaviour
{
    static List<GameMonoBehaviour> _registeredBehaviours = new List<GameMonoBehaviour>();
    public static ReadOnlyList<GameMonoBehaviour> RegisteredBehaviours => _registeredBehaviours.AsReadOnlyNoAlloc();

    protected virtual void Awake()
    {
        if (Game.Ready)
            OnGameReady();
        if (Game.Started)
            OnGameStart();
        _registeredBehaviours.Add(this);
        if (Game.Ready)
            OnGameReady();
        if (Game.Started)
            OnGameStart();
    }

    protected virtual void OnDestroy()
    {
        if(ApplicationUtilityService.ApplicationIsQuitting == false)
        {
            _registeredBehaviours.Remove(this);
            OnSafeDestroy();
        }
    }

    public virtual void OnGameUpdate() { }
    public virtual void OnGameFixedUpdate() { }
    public virtual void OnGameReady() { }
    public virtual void OnGameStart() { }
    public virtual void OnSafeDestroy() { }
}
