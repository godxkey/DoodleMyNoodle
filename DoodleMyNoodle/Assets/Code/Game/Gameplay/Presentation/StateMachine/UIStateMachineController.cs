using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UIStateMachineController : GamePresentationSystem<UIStateMachineController>
{
    public UIState CurrentSate => StateMachine.CurrentState;
    public UIStateMachine StateMachine { get; private set; } = new UIStateMachine();

    protected override void OnGamePresentationUpdate()
    {
        StateMachine.Blackboard.SimWorld = SimWorld;
        StateMachine.Blackboard.Cache = Cache;

        if (Cache.LocalController != Entity.Null)
        {
            if (StateMachine.CurrentState == null)
            {
                StateMachine.TransitionTo(UIStateType.Gameplay);
            }
        }

        StateMachine.Update();
    }
}

public class UIStateMachine : StateMachine<UIStateMachine, UIState, UIStateMachineBlackboard>
{
    public static UIStateMachine Instance => UIStateMachineController.Instance?.StateMachine;

    public void TransitionTo(UIStateType type, object inputData = null)
    {
        switch (type)
        {
            case UIStateType.Gameplay:
                TransitionTo(Blackboard.GameplayState, inputData);
                break;

            case UIStateType.BlockedGameplay:
                TransitionTo(Blackboard.BlockedGameplayState, inputData);
                break;

            case UIStateType.ParameterSelection:
                TransitionTo(Blackboard.ParameterSelectionState, inputData);
                break;

            default:
                throw new NotImplementedException();
        }
    }
}
