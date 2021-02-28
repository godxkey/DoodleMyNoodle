public class BlockedGameplayState : UIState
{
    public override UIStateType Type => UIStateType.BlockedGameplay;

    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
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
