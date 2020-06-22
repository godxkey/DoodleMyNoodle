using UnityEngine;
using System.Collections.Generic;
using UnityEngineX;

/// <summary>
/// A GameMonoBehaviour is a monobehaviour class that receives common game events
/// </summary>
public class GameMonoBehaviour : MonoBehaviour, IIndexedInList
{
    static List<GameMonoBehaviour> s_registeredBehaviours = new List<GameMonoBehaviour>();
    public static ReadOnlyList<GameMonoBehaviour> RegisteredBehaviours => s_registeredBehaviours.AsReadOnlyNoAlloc();

    int IIndexedInList.Index { get; set; }

    protected virtual void Awake()
    {
        s_registeredBehaviours.AddIndexed(this);

        if (Game.Ready)
            OnGameAwake();
        if (Game.Started)
            OnGameStart();
    }

    protected virtual void OnDestroy()
    {
        s_registeredBehaviours.RemoveIndexed(this);

        if (ApplicationUtilityService.ApplicationIsQuitting == false)
        {
            OnSafeDestroy();
        }
    }

    public virtual void OnGameUpdate() { }
    public virtual void OnGameFixedUpdate() { }
    public virtual void OnGameLateUpdate() { }
    public virtual void OnGameAwake() { }
    public virtual void OnGameStart() { }
    public virtual void OnSafeDestroy() { }
}
