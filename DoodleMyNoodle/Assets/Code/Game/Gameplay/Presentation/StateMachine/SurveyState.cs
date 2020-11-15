using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SurveyState : UIState
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
                GameObject SurveyPrefab = itemAuth.FindSurveyDefinitionForParameters(ParameterDescriptionList);

                if (SurveyPrefab != null)
                {
                    SurveyManager.Instance.RequestData(Cache.LocalPawnPositionFloat, SurveyPrefab,
                   (List<GameAction.ParameterData> Results) =>
                   {
                       UIStateMachine.Instance.TransitionTo(StateTypes.ParameterSelection, GetData<Entity>(0), Results);
                   }, ParameterDescriptionList);
                }
                else
                {
                    // Default Case

                    // For default case, we handle them one at a time with the survey corresponding to the first parameter we have
                    GameAction.ParameterDescription ParameterToHandle = ParameterDescriptionList[0];
                    SurveyManager.Instance.RequestDataWithDefaultSurvey(Cache.LocalPawnPositionFloat, ParameterToHandle,
                   (List<GameAction.ParameterData> Results) =>
                   {
                       UIStateMachine.Instance.TransitionTo(StateTypes.ParameterSelection, GetData<Entity>(0), Results);
                   });
                }
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

    public override StateTypes StateType => StateTypes.Survey;

    public override bool IsTransitionValid(StateTypes newState)
    {
        return true;
    }
}