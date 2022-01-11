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

        StateMachine.Update();
    }
}

public class UIStateMachine : StateMachine<UIStateMachine, UIState, UIStateMachineBlackboard>
{
    public static UIStateMachine Instance => UIStateMachineController.Instance?.StateMachine;

    public GamePresentationCache Cache => Blackboard.Cache;
    public ExternalSimWorldAccessor SimWorld => Blackboard.SimWorld;

    public override void Update()
    {
        base.Update();

        // todo : something better for when a system manually transition to state and wanna keep handling manually
        if (PromptDisplay.Instance.IsWaitingForAnswer)
        {
            return;
        }

        bool couldPlay =
            Cache.LocalControllerExists
            && Cache.LocalPawnAlive
            && Cache.CanLocalPlayerPlay;

        UIStateType? currentType = CurrentState?.Type;

        if (couldPlay)
        {
            if (currentType == null || currentType == UIStateType.BlockedGameplay)
                TransitionTo(UIStateType.Gameplay);
        }
        else
        {
            if (currentType == null || currentType == UIStateType.Gameplay)
                TransitionTo(UIStateType.BlockedGameplay);
        }
    }

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

            case UIStateType.Drawing:
                TransitionTo(Blackboard.DrawingState, inputData);
                break;

            default:
                throw new NotImplementedException();
        }
    }
}
