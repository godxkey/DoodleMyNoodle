public class GameplayState : UIState
{
    public override void OnEnter()
    {
        PlayerActionBarDisplay.Instance.EnableInteraction();
    }

    public override void OnUpdate()
    {
        if (!CommonReads.CanTeamPlay(SimWorld, SimWorld.GetComponentData<Team>(Cache.LocalController))) 
        {
            UIStateMachine.Instance.TransitionTo(StateTypes.BlockedGameplay);
        }
    }

    public override void OnExit(StateTypes newState)
    {
        PlayerActionBarDisplay.Instance.BlockInteraction();
    }

    public override StateTypes GetStateType()
    {
        return StateTypes.Gameplay;
    }

    public override bool IsTransitionValid(StateTypes newState)
    {
        return true;
    }
}
