using Unity.Entities;

public class MiniGameState : UIState
{
    public override void OnEnter()
    {
        Entity itemEntity = GetData<Entity>(0);
        if (SimWorld.TryGetComponentData(itemEntity, out SimAssetId itemSimAssetID))
        {
            ItemAuth itemAuth = ItemInfoBank.Instance.GetItemAuthFromID(itemSimAssetID);
            if (itemAuth != null)
            {
                MiniGameManager.Instance.TriggerMiniGame(GetData<GameActionParameterMiniGame.Description>(1), itemAuth.MiniGame, 
                (GameAction.ParameterData MiniGameResultData) =>
                {
                    UIStateMachine.Instance.TransitionTo(StateTypes.ParameterSelection, GetData<Entity>(0), MiniGameResultData);
                });
            }
            else
            {
                UIStateMachine.Instance.TransitionTo(StateTypes.Gameplay);
            }
        }
        else
        {
            UIStateMachine.Instance.TransitionTo(StateTypes.Gameplay);
        }
    }

    public override void OnUpdate() { }

    public override void OnExit(StateTypes newState) 
    { 
        // todo : cancel mini game
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