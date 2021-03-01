using System;
using UnityEngine;
using UnityEngineX;

// default basic state
public abstract class State<TBlackboard> : State where TBlackboard : class, new()
{
    public new TBlackboard Blackboard => (TBlackboard)StateMachine.Blackboard;
}

// default basic state machine
public class StateMachine<TBlackboard> : StateMachine
    where TBlackboard : class, new()
{
    public StateMachine(TBlackboard blackboard)
    {
        Blackboard = blackboard ?? throw new ArgumentNullException(nameof(blackboard));
    }

    public StateMachine()
    {
        Blackboard = new TBlackboard();
    }

    public new TBlackboard Blackboard { get => (TBlackboard)base.Blackboard; set => base.Blackboard = value; }
}

public abstract class State<TStateMachine, TState, TBlackboard> : State
    where TStateMachine : StateMachine<TStateMachine, TState, TBlackboard>
    where TState : State<TStateMachine, TState, TBlackboard>
    where TBlackboard : class, new()
{
    public new TStateMachine StateMachine => (TStateMachine)base.StateMachine;
    public new TBlackboard Blackboard => StateMachine.Blackboard;

    public sealed override void OnExit(State nextState) { OnExit((TState)nextState); }
    public abstract void OnExit(TState nextState);
}

public class StateMachine<TStateMachine, TState, TBlackboard> : StateMachine
    where TStateMachine : StateMachine<TStateMachine, TState, TBlackboard>
    where TState : State<TStateMachine, TState, TBlackboard>
    where TBlackboard : class, new()
{
    public StateMachine(TBlackboard blackboard)
    {
        Blackboard = blackboard ?? throw new ArgumentNullException(nameof(blackboard));
    }

    public StateMachine()
    {
        Blackboard = new TBlackboard();
    }

    public new TState CurrentState { get => (TState)base.CurrentState; }
    public new TBlackboard Blackboard { get => (TBlackboard)base.Blackboard; set => base.Blackboard = value; }
}

public abstract class State
{
    public StateMachine StateMachine { get; private set; }
    public object Blackboard => StateMachine.Blackboard;

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit(State nextState);

    // internals
    public virtual Type ExpectedInputParameterType => typeof(object);
    public object InputParameter { get; set; }
    public void InternalSetOwner(StateMachine owner)
    {
        if (StateMachine != null && StateMachine != owner)
            throw new Exception("State is being used for multiple state machines at the same time. This is not supported");
        StateMachine = owner;
    }
}

public class StateMachine
{
    private bool _exiting;

    public StateMachine(object blackboard)
    {
        Blackboard = blackboard ?? throw new ArgumentNullException(nameof(blackboard));
    }

    public StateMachine()
    {
    }

    public State CurrentState { get; private set; }
    public object Blackboard { get; set; }

    public void TransitionTo(State newState, object inputParameter = null)
    {
        if (_exiting)
        {
            Log.Error("Cannot transition to a new state while we are exiting from one. Do not call TransitionTo(..) from an OnExit method");
            return;
        }

        if (CurrentState != null)
        {
            _exiting = true;
            CurrentState.OnExit(nextState: newState);
            _exiting = false;
        }

        CurrentState = newState;

        if (CurrentState != null)
        {
            if (inputParameter != null && !CurrentState.ExpectedInputParameterType.IsAssignableFrom(inputParameter.GetType()))
            {
                throw new Exception($"{CurrentState.GetType().GetPrettyName()} expects input parameter of type {CurrentState.ExpectedInputParameterType.GetPrettyName()}, not {inputParameter.GetType().GetPrettyName()}.");
            }
            CurrentState.InputParameter = inputParameter;
            CurrentState.InternalSetOwner(this);
            CurrentState.OnEnter();
        }
    }

    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.OnUpdate();
        }
    }
}