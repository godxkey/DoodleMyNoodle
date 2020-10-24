using System.Collections.Generic;
using Unity.Entities;

public class UIStateMachine : GamePresentationSystem<UIStateMachine>
{
    private UIState _currentState;
    public UIState CurrentSate => _currentState;

    private Dictionary<UIState.StateTypes, UIState> _stateInstances = new Dictionary<UIState.StateTypes, UIState>(); 

    protected override void Awake()
    {
        base.Awake();

        _currentState = GetStateInstance(UIState.StateTypes.Gameplay);
        _currentState.OnEnter();
    }

    protected override void OnGamePresentationUpdate() 
    {
        if (Cache.LocalController != Entity.Null)
        {
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

    private UIState GetStateInstance(UIState.StateTypes newStat, params object[] stateData)
    {
        if (_stateInstances.ContainsKey(newStat))
        {
            // if someone want to transition to a state that already exist and wish to push its data, overwrite
            // the data that currently exist in state with new one
            if (stateData.Length > 0)
            {
                _stateInstances[newStat].Data = stateData;
            }

            return _stateInstances[newStat];
        }
        else
        {
            UIState newStateInstance = null;

            switch (newStat)
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
                case UIState.StateTypes.TileSelection:
                    newStateInstance = new TileSelectionState();
                    break;
                case UIState.StateTypes.EntitySelection:
                    newStateInstance = new EntitySelectionState();
                    break;
                case UIState.StateTypes.MiniGame:
                    newStateInstance = new MiniGameState();
                    break;
                default:
                    break;
            }

            newStateInstance.Data = stateData;
            newStateInstance.Cache = Cache;
            newStateInstance.SimWorld = SimWorld;

            _stateInstances.Add(newStat, newStateInstance);

            return newStateInstance;
        }
    }
}
