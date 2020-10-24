using Unity.Entities;

public class TileSelectionState : UIState
{
    public override void OnEnter()
    {
        TileHighlightManager.Instance.AskForSingleTileSelectionAroundPlayer(GetData<GameActionParameterTile.Description>(1), (GameActionParameterTile.Data TileSelectedData) =>
        {
             UIStateMachine.Instance.TransitionTo(StateTypes.ParameterSelection, GetData<Entity>(0), TileSelectedData);
        });
    }

    public override void OnUpdate() { }

    public override void OnExit(StateTypes newState)
    {
        TileHighlightManager.Instance.InterruptTileSelectionProcess();
    }

    public override StateTypes GetStateType()
    {
        return StateTypes.TileSelection;
    }

    public override bool IsTransitionValid(StateTypes newState)
    {
        return true;
    }
}
