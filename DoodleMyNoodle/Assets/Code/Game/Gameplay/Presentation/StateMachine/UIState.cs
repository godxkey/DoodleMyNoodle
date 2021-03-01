using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

// the blackboard is meant to be used as a hub of shared information (across all UI states). It can be modified as needed.
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