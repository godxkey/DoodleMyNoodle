public class BlockedGameplayState : UIState
{
    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        if (CommonReads.CanTeamPlay(SimWorld, SimWorld.GetComponentData<Team>(Cache.LocalController)))
        {
            UIStateMachine.Instance.TransitionTo(StateTypes.Gameplay);
        }
    }

    public override void OnExit(StateTypes newState)
    {

    }

    public override StateTypes GetStateType()
    {
        return StateTypes.BlockedGameplay;
    }

    public override bool IsTransitionValid(StateTypes newState)
    {
        return true;
    }
}
