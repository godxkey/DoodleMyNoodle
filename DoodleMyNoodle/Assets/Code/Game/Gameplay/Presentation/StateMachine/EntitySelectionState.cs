public class EntitySelectionState : UIState
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

    public override StateTypes StateType => StateTypes.EntitySelection;

    public override bool IsTransitionValid(StateTypes newState)
    {
        return true;
    }
}
