using System.Collections.Generic;
using Unity.Entities;

public class GameActionRequestState : UIState
{
    public override void OnEnter()
    {
        Entity itemEntity = GetData<Entity>(0);
        if (SimWorld.TryGetComponentData(itemEntity, out SimAssetId itemSimAssetID))
        {
            ItemAuth itemAuth = ItemInfoBank.Instance.GetItemAuthFromID(itemSimAssetID);
            if (itemAuth != null)
            {
                GameAction.ParameterDescription[] ParameterDescriptionList = GetData<GameAction.ParameterDescription[]>(1);
                GameActionRequestManager.Instance.RequestData(Cache.LocalPawnPosition.ToUnityVec(), itemAuth.FindRequestDefinitionForParameters(ParameterDescriptionList),
                (List<GameAction.ParameterData> Results) =>
                {
                    UIStateMachine.Instance.TransitionTo(StateTypes.ParameterSelection, GetData<Entity>(0), Results);
                }, ParameterDescriptionList);
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

    public override StateTypes StateType => StateTypes.GameActionRequest;

    public override bool IsTransitionValid(StateTypes newState)
    {
        return true;
    }
}