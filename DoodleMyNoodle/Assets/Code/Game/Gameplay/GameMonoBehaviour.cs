using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class GameMonoBehaviour : MonoBehaviour
{
    static List<GameMonoBehaviour> _registeredBehaviours = new List<GameMonoBehaviour>();
    public static ReadOnlyCollection<GameMonoBehaviour> registeredBehaviours = _registeredBehaviours.AsReadOnly();

    protected virtual void Awake()
    {
        _registeredBehaviours.Add(this);
    }

    protected virtual void OnDestroy()
    {
        if(ApplicationUtilityService.ApplicationIsQuitting == false)
        {
            _registeredBehaviours.Remove(this);
            OnSafeDestroy();
        }
    }

    public virtual void OnGamePreReady() { }
    public virtual void OnGameReady() { }
    public virtual void OnSafeDestroy() { }
}
