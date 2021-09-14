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
        public Entity ObjectEntity;
        public bool IsItem;
        public int ItemIndex;
    }

    private SurveyBlackboard _surveySMBlackboard = new SurveyBlackboard();
    private StateMachine _surveyStateMachine = new StateMachine();

    public override UIStateType Type => UIStateType.ParameterSelection;

    public override void OnEnter()
    {
        _surveyStateMachine.Blackboard = _surveySMBlackboard;
        _surveySMBlackboard.Cache = Cache;
        _surveySMBlackboard.ResultParameters.Clear();
        _surveySMBlackboard.isDebug = false;

        CursorOverlayService.Instance.ResetCursorToDefault();

        if (InputParameter != null && InputParameter.ObjectEntity != Entity.Null)
        {
            if (SimWorld.TryGetComponent(InputParameter.ObjectEntity, out SimAssetId objectSimAssetID))
            {
                _surveySMBlackboard.GameActionAuth = PresentationHelpers.FindItemAuth(objectSimAssetID);
            }

            if (_surveySMBlackboard.GameActionAuth == null)
            {
                StateMachine.TransitionTo(UIStateType.Gameplay);
                return;
            }

            // Init process of parameter selection
            GameActionId actionId = SimWorld.GetComponent<GameActionId>(InputParameter.ObjectEntity);
            GameAction objectGameAction = GameActionBank.GetAction(actionId);

            GameAction.UseContext useContext = new GameAction.UseContext()
            {
                InstigatorPawn = Cache.LocalPawn,
                InstigatorPawnController = Cache.LocalController,
                Item = InputParameter.ObjectEntity
            };

            _surveySMBlackboard.UseContext = useContext;
            _surveySMBlackboard.ParametersDescriptions = objectGameAction.GetUseContract(SimWorld, useContext).ParameterTypes;
        }
        else
        {
            _surveySMBlackboard.isDebug = true;
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

        if (_surveySMBlackboard.isDebug)
        {
            _surveyStateMachine.Update();
            return;
        }

        if (!SimWorld.Exists(InputParameter.ObjectEntity)) // target item no longer exists, exit
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
            SimPlayerInputUseItem simInput = new SimPlayerInputUseItem(InputParameter.ItemIndex, _surveySMBlackboard.ResultParameters);
            SimWorld.SubmitInput(simInput);
        }
        else
        {
            fix2 entityPosition = SimWorld.GetComponent<FixTranslation>(InputParameter.ObjectEntity);

            SimPlayerInputUseObjectGameAction simInput = new SimPlayerInputUseObjectGameAction(entityPosition, _surveySMBlackboard.ResultParameters);
            SimWorld.SubmitInput(simInput);
        }

        StateMachine.TransitionTo(UIStateType.Gameplay);
    }


    private class SurveyBlackboard
    {
        public ItemAuth GameActionAuth;
        public GamePresentationCache Cache;

        public bool isDebug;

        public GameAction.UseContext UseContext;

        // the description of parameters we must fill
        public GameAction.ParameterDescription[] ParametersDescriptions;

        // the resulting param data
        public List<GameAction.ParameterData> ResultParameters = new List<GameAction.ParameterData>();
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

            if (Blackboard.isDebug)
                return;

            // the parameter
            int remainingParamCount = Blackboard.ParametersDescriptions.Length - Blackboard.ResultParameters.Count;

            GameAction.ParameterDescription[] remainingParams = ArrayX.SubArray(Blackboard.ParametersDescriptions, 0, remainingParamCount);

            if(remainingParams.Length < 1)
            {
                OnSurveyCancel();
                return;
            }

            SurveyBaseController surveyPrefab = Blackboard.GameActionAuth.FindCustomSurveyPrefabForParameters(remainingParams);
            if (surveyPrefab != null)
            {
                SurveyManager.Instance.BeginSurvey(Blackboard.Cache.LocalPawnPositionFloat, Blackboard.UseContext, remainingParams, surveyPrefab, OnSurveyComplete, OnSurveyCancel);
            }
            else
            {
                // Default Case

                // For default case, we handle them one at a time with the survey corresponding to the first parameter we have
                GameAction.ParameterDescription parameterToHandle = remainingParams[0];
                SurveyManager.Instance.BeginDefaultSurvey(Blackboard.Cache.LocalPawnPositionFloat, Blackboard.UseContext, parameterToHandle, OnSurveyComplete, OnSurveyCancel);
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





