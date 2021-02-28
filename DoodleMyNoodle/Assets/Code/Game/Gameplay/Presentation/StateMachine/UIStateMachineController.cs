using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UIStateMachineController : GamePresentationSystem<UIStateMachineController>
{
    public UIState CurrentSate => StateMachine.CurrentState;
    public UIStateMachine StateMachine { get; private set; } = new UIStateMachine();

    public new static UIStateMachine Instance => GamePresentationSystem<UIStateMachineController>.Instance.StateMachine;

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
