using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UIStateMachine : GamePresentationSystem<UIStateMachine>
{
    private UIState _currentState = null;
    public UIState CurrentSate => _currentState;

    private Dictionary<UIState.StateTypes, UIState> _stateInstances = new Dictionary<UIState.StateTypes, UIState>(); 

    protected override void OnGamePresentationUpdate() 
    {
        if (Cache.LocalController != Entity.Null)
        {
            if (_currentState == null)
            {
                _currentState = GetStateInstance(UIState.StateTypes.Gameplay);
                _currentState.OnEnter();
            }

            _currentState.OnUpdate();
        }
    }

    public void TransitionTo(UIState.StateTypes newState, params object[] stateData)
    {
        if (_currentState.IsTransitionValid(newState))
        {
            _currentState.OnExit(newState);

            _currentState = GetStateInstance(newState, stateData);

            _currentState.OnEnter();
        }
    }

    private UIState GetStateInstance(UIState.StateTypes newState, params object[] stateData)
    {
        if (_stateInstances.TryGetValue(newState, out UIState uIState))
        {
            // if someone want to transition to a state that already exist and wish to push its data, overwrite
            // the data that currently exist in state with new one
            if (stateData.Length > 0)
            {
                uIState.Data = stateData;
            }

            return uIState;
        }
        else
        {
            UIState newStateInstance = null;

            switch (newState)
            {
                case UIState.StateTypes.Gameplay:
                    newStateInstance = new GameplayState();
                    break;
                case UIState.StateTypes.BlockedGameplay:
                    newStateInstance = new BlockedGameplayState();
                    break;
                case UIState.StateTypes.ParameterSelection:
                    newStateInstance = new ParameterSelectionState();
                    break;
                case UIState.StateTypes.GameActionRequest:
                    newStateInstance = new GameActionRequestState();
                    break;
                default:
                    throw new NotImplementedException();
            }

            newStateInstance.Data = stateData;
            newStateInstance.Cache = Cache;
            newStateInstance.SimWorld = SimWorld;

            _stateInstances.Add(newState, newStateInstance);

            return newStateInstance;
        }
    }
}
