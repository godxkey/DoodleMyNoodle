public class DrawingState : UIState
{
    public override UIStateType Type => UIStateType.Drawing;

    public override void OnEnter()
    {
        // disable interactions. We need to provide a key, we use our state name.
        PlayerActionBarDisplay.Instance.DisableInteraction(cause: Name);
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit(UIState newState)
    {
        // re-enable interactions. We need to provide the same key (our state name).
        PlayerActionBarDisplay.Instance.UndoDisableInteraction(cause: Name);
    }
}