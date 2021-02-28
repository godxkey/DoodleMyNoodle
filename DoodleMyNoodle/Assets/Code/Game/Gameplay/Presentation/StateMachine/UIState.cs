using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class UIStateMachineBlackboard
{
    public GamePresentationCache Cache;
    public ExternalSimWorldAccessor SimWorld;

    // state instances
    public GameplayState GameplayState = new GameplayState();
    public ParameterSelectionState ParameterSelectionState = new ParameterSelectionState();
    public BlockedGameplayState BlockedGameplayState = new BlockedGameplayState();
}

public enum UIStateType
{
    Gameplay,
    BlockedGameplay,
    ParameterSelection,
}

public abstract class UIState<TInputDataType> : UIState
{
    public new TInputDataType InputParameter => (TInputDataType)base.InputParameter;
}

public abstract class UIState : State<UIStateMachine, UIState, UIStateMachineBlackboard>
{
    public GamePresentationCache Cache => Blackboard.Cache;
    public ExternalSimWorldAccessor SimWorld => Blackboard.SimWorld;

    public abstract UIStateType Type { get; }
}

public class UIStateMachine : StateMachine<UIStateMachine, UIState, UIStateMachineBlackboard>
{
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