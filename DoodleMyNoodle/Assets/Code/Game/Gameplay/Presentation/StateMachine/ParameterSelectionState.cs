using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using UnityEngine;
using System;
using UnityEngineX;

public class ParameterSelectionState : UIState<ParameterSelectionState.InputParam>
{
    public class InputParam
    {
        public Entity ActionInstigator;
        public Entity ActionPrefab;
        public bool IsItem;
        public int ItemIndex;
    }

    private SurveyBlackboard _surveySMBlackboard = new SurveyBlackboard();
    private StateMachine _surveyStateMachine = new StateMachine();

    public override UIStateType Type => UIStateType.ParameterSelection;

    public override void OnEnter()
    {
        _surveyStateMachine.Blackboard = _surveySMBlackboard;
        _surveySMBlackboard.Reset();
        _surveySMBlackboard.Cache = Cache;

        CursorOverlayService.Instance.ResetCursorToDefault();

        if (InputParameter != null && InputParameter.ActionInstigator != Entity.Null && InputParameter.ActionPrefab != Entity.Null)
        {
            if (SimWorld.TryGetComponent(InputParameter.ActionPrefab, out SimAssetId objectSimAssetID))
            {
                _surveySMBlackboard.ActionAuth = PresentationHelpers.FindActionAuth(objectSimAssetID);
            }

            // Init process of parameter selection
            GameActionId actionId = SimWorld.GetComponent<GameActionId>(InputParameter.ActionPrefab);
            GameAction objectGameAction = GameActionBank.GetAction(actionId);

            _surveySMBlackboard.UseContext = CommonReads.GetActionContext(SimWorld, InputParameter.ActionInstigator, InputParameter.ActionPrefab);
            _surveySMBlackboard.ParametersDescriptions = objectGameAction.GetExecutionContract(SimWorld, InputParameter.ActionPrefab).ParameterTypes;
        }

        if (_surveySMBlackboard.ActionAuth == null)
        {
            Log.Warning($"No action auth found for item. Returning to UI state gameplay.");
            StateMachine.TransitionTo(UIStateType.Gameplay);
            return;
        }

        if (_surveySMBlackboard.ParametersDescriptions == null)
        {
            StateMachine.TransitionTo(UIStateType.Gameplay);
            return;
        }

        _surveyStateMachine.TransitionTo(new SurveyState());
    }

    public override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(1))  // right-click cancels
        {
            StateMachine.TransitionTo(UIStateType.Gameplay);
            return;
        }

        if (!SimWorld.Exists(InputParameter.ActionInstigator)) // target item no longer exists, exit
        {
            StateMachine.TransitionTo(UIStateType.Gameplay);
            return;
        }

        // No currently running survey ? (or current survey done)
        if (_surveyStateMachine.CurrentState == null || ((SurveyState)_surveyStateMachine.CurrentState).Done)
        {
            bool isFinished = _surveySMBlackboard.ResultParameters.Count == _surveySMBlackboard.ParametersDescriptions.Length;
            if (isFinished)
            {
                // all done! send input and exit
                FinishAndSendSimInput();
            }
            else
            {
                // not done, begin next survey state ?
                if (_surveySMBlackboard.ResultParameters.Count == 0)
                {
                    // no data found, cancel and go back to gameplay
                    StateMachine.TransitionTo(UIStateType.Gameplay);
                    return;
                }
                else
                {
                    // begin next survey state
                    _surveyStateMachine.TransitionTo(new SurveyState());
                }
            }

        }

        _surveyStateMachine.Update();
    }

    public override void OnExit(UIState newState)
    {
        _surveyStateMachine.TransitionTo(null); // exit of current state
    }

    private void FinishAndSendSimInput()
    {
        // process completed, we have all info let's use the game action
        if (InputParameter.IsItem)
        {
            _surveySMBlackboard.ResultParameters.RemoveNulls();
            SimPlayerInputUseItem simInput = new SimPlayerInputUseItem(InputParameter.ItemIndex, _surveySMBlackboard.ResultParameters);
            SimWorld.SubmitInput(simInput);
        }

        StateMachine.TransitionTo(UIStateType.Gameplay);
    }


    private class SurveyBlackboard
    {
        public GameActionAuth ActionAuth;
        public GamePresentationCache Cache;
        public GameAction.ExecutionContext UseContext;

        // the description of parameters we must fill
        public GameAction.ParameterDescription[] ParametersDescriptions;

        // the resulting param data
        public List<GameAction.ParameterData> ResultParameters = new List<GameAction.ParameterData>();

        public void Reset()
        {
            Cache = null;
            ResultParameters.Clear();
            ActionAuth = null;
            UseContext = default;
            ParametersDescriptions = default;
        }
    }

    private class SurveyState : State<SurveyBlackboard>
    {
        public bool Done;

        private bool _doneNextFrame = false;

        public override void OnEnter()
        {
            Done = false;
            _doneNextFrame = false;

            HUDDisplay.Instance.ToggleVisibility(false);

            // the parameter
            int remainingParamCount = Blackboard.ParametersDescriptions.Length - Blackboard.ResultParameters.Count;

            GameAction.ParameterDescription[] remainingParams = ArrayX.SubArray(Blackboard.ParametersDescriptions, Blackboard.ResultParameters.Count, remainingParamCount);

            if (remainingParams.Length < 1)
            {
                OnSurveyCancel();
                return;
            }

            SurveyBaseController surveyPrefab = Blackboard.ActionAuth.FindCustomSurveyPrefabForParameters(remainingParams);
            if (surveyPrefab != null)
            {
                SurveyManager.Instance.BeginSurvey(Blackboard.Cache.LocalPawnPositionFloat, Blackboard.UseContext, Blackboard.ResultParameters, remainingParams, surveyPrefab, OnSurveyComplete, OnSurveyCancel);
            }
            else
            {
                // default case
                Blackboard.ResultParameters.Add(null);
                _doneNextFrame = true;

                //GameAction.ParameterDescription parameterToHandle = remainingParams[0];
                //SurveyManager.Instance.BeginDefaultSurvey(Blackboard.Cache.LocalPawnPositionFloat, Blackboard.UseContext, Blackboard.ResultParameters, parameterToHandle, OnSurveyComplete, OnSurveyCancel);
            }
        }

        private void OnSurveyCancel()
        {
            _doneNextFrame = true;
        }

        private void OnSurveyComplete(List<GameAction.ParameterData> results)
        {
            // add param to results
            Blackboard.ResultParameters.AddRange(results);
            _doneNextFrame = true;
        }

        public override void OnUpdate()
        {
            if (_doneNextFrame) // this is needed to make sure we don't start the next survey in the same frame as the previous
            {
                _doneNextFrame = false;
                Done = true;
            }
        }

        public override void OnExit(State nextState)
        {
            HUDDisplay.Instance.ToggleVisibility(true);
            SurveyManager.Instance.StopCurrentSurvey();
        }
    }
}





