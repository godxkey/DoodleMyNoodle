public class BlockedGameplayState : UIState
{
    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        if (CommonReads.CanTeamPlay(SimWorld, SimWorld.GetComponentData<Team>(Cache.LocalController)))
        {
            if (SimWorld.GetComponentData<Health>(Cache.LocalPawn).Value > 0)
            {
                UIStateMachine.Instance.TransitionTo(StateTypes.Gameplay);
            }
        }
    }

    public override void OnExit(StateTypes newState)
    {

    }

    public override StateTypes StateType => StateTypes.BlockedGameplay;

    public override bool IsTransitionValid(StateTypes newState)
    {
        return true;
    }
}
