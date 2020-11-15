using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIState
{
    public enum StateTypes
    {
        Gameplay,
        BlockedGameplay,
        ParameterSelection,
        Survey
    }

    public object[] Data;
    public GamePresentationCache Cache;
    public ExternalSimWorldAccessor SimWorld;

    protected T GetData<T>(int index) { return (T)Data[index]; }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit(StateTypes newState);

    public abstract bool IsTransitionValid(StateTypes newState);
    public abstract StateTypes StateType { get; }
}