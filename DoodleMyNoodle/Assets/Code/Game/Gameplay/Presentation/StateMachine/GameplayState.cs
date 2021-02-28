public class GameplayState : UIState
{
    public override UIStateType Type => UIStateType.Gameplay;

    public override void OnEnter()
    {
        PlayerActionBarDisplay.Instance.EnableInteraction();
    }

    public override void OnUpdate()
    {
        if (!CommonReads.CanTeamPlay(SimWorld, SimWorld.GetComponentData<Team>(Cache.LocalController))) 
        {
            StateMachine.TransitionTo(UIStateType.BlockedGameplay);
        }
        else
        {
            if (SimWorld.GetComponentData<Health>(Cache.LocalPawn).Value <= 0)
            {
                StateMachine.TransitionTo(UIStateType.BlockedGameplay);
            }
        }
    }

    public override void OnExit(UIState newState)
    {
        PlayerActionBarDisplay.Instance.BlockInteraction();
    }
}
