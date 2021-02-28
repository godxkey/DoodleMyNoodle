public class BlockedGameplayState : UIState
{
    public override UIStateType Type => UIStateType.BlockedGameplay;

    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        // todo : something better for when a system manually transition to state and wanna keep handling manually
        if (PromptDisplay.Instance.IsWaitingForAnswer)
        {
            return;
        }

        if (CommonReads.CanTeamPlay(SimWorld, SimWorld.GetComponentData<Team>(Cache.LocalController)))
        {
            if (SimWorld.GetComponentData<Health>(Cache.LocalPawn).Value > 0)
            {
                StateMachine.TransitionTo(Blackboard.GameplayState);
            }
        }
    }

    public override void OnExit(UIState newState)
    {

    }
}
