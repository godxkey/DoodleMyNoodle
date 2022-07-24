﻿using System.Collections.Generic;
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
        public Entity ItemEntity;
        public Entity ActionPrefab;
        public bool IsItem;
        public int ItemIndex;
        public KeyCode PressedKey;
        public Action OnFinishOrCancelCallback;
    }
    public List<GameAction.ParameterData> ResultParameters = new List<GameAction.ParameterData>();
    private bool _doneNextFrame;

    public override UIStateType Type => UIStateType.ParameterSelection;

    public override void OnEnter()
    {
        CursorOverlayService.Instance.ResetCursorToDefault();

        SurveyBaseController surveyPrefab = null;
        GameActionAuth actionAuth = null;
        GameAction.ExecutionContext useContext = default;
        GameAction.ParameterDescription[] parametersDescriptions = null;
        ResultParameters.Clear();

        _doneNextFrame = true;
        if (InputParameter != null && InputParameter.ItemEntity != Entity.Null && InputParameter.ActionPrefab != Entity.Null)
        {
            // Init process of parameter selection
            GameActionId actionId = SimWorld.GetComponent<GameActionId>(InputParameter.ActionPrefab);
            GameAction objectGameAction = GameActionBank.GetAction(actionId);

            useContext = CommonReads.GetActionContext(SimWorld, InputParameter.ItemEntity, InputParameter.ActionPrefab);
            var execContract = objectGameAction.GetExecutionContract(SimWorld, InputParameter.ActionPrefab);
            parametersDescriptions = execContract?.ParameterTypes;

            if (SimWorld.TryGetComponent(InputParameter.ItemEntity, out SimAssetId itemSimAsset))
            {
                var itemAuth = PresentationHelpers.FindItemAuth(itemSimAsset);

                if (itemAuth != null)
                {
                    CursorOverlayService.Instance.SetCursor(new CursorOverlayService.CursorSetting()
                    {
                        ClickedIcon = itemAuth.Icon,
                        Icon = itemAuth.Icon,
                        Type = CursorOverlayService.CursorType.Default,
                        Scale = 3f
                    });
                }
            }

            if (SimWorld.TryGetComponent(InputParameter.ActionPrefab, out SimAssetId objectSimAssetID))
            {
                actionAuth = PresentationHelpers.FindActionAuth(objectSimAssetID);

                if (actionAuth != null)
                {
                    surveyPrefab = actionAuth.Surveys.Count > 0 ? actionAuth.Surveys[0] : null;

                    var localPawnViewTransform = PresentationHelpers.FindBindedView(Blackboard.Cache.LocalPawn)?.transform;
                    SurveyManager.Instance.BeginSurvey(
                        InputParameter.PressedKey,
                        localPawnViewTransform,
                        useContext,
                        ResultParameters,
                        parametersDescriptions,
                        surveyPrefab,
                        OnSurveyComplete,
                        OnSurveyCancel);
                    _doneNextFrame = false;
                }
            }
        }

        if (actionAuth == null)
        {
            Log.Warning($"No action auth found for item. Returning to UI state gameplay.");
            StateMachine.TransitionTo(UIStateType.Gameplay);
            return;
        }
    }

    private void OnSurveyCancel()
    {
        _doneNextFrame = true;
    }

    private void OnSurveyComplete(List<GameAction.ParameterData> results)
    {
        // add param to results
        ResultParameters.AddRange(results);
        _doneNextFrame = true;
    }

    public override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(1))  // right-click cancels
        {
            StateMachine.TransitionTo(UIStateType.Gameplay);
            return;
        }

        if (!SimWorld.Exists(InputParameter.ItemEntity)) // target item no longer exists, exit
        {
            StateMachine.TransitionTo(UIStateType.Gameplay);
            return;
        }

        if (_doneNextFrame)
        {
            FinishAndSendSimInput();
            return;
        }
    }

    public override void OnExit(UIState newState)
    {
        CursorOverlayService.Instance.ResetCursorToDefault();
        InputParameter.OnFinishOrCancelCallback?.Invoke();
        InputParameter.OnFinishOrCancelCallback = null;
        SurveyManager.Instance.StopCurrentSurvey();
    }

    private void FinishAndSendSimInput()
    {
        // process completed, we have all info let's use the game action
        if (InputParameter.IsItem)
        {
            ResultParameters.RemoveNulls();
            SimPlayerInputUseItem simInput = new SimPlayerInputUseItem(InputParameter.ItemIndex, ResultParameters);
            SimWorld.SubmitInput(simInput);
        }

        StateMachine.TransitionTo(UIStateType.Gameplay);
        InputParameter.OnFinishOrCancelCallback?.Invoke();
        InputParameter.OnFinishOrCancelCallback = null;
    }
}





