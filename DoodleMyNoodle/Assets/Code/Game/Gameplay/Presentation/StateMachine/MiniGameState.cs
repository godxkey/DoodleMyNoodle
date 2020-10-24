public class MiniGameState : UIState
{
    public override void OnEnter()
    {

    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(StateTypes newState)
    {

    }

    public override StateTypes GetStateType()
    {
        return StateTypes.MiniGame;
    }

    public override bool IsTransitionValid(StateTypes newState)
    {
        return true;
    }
}