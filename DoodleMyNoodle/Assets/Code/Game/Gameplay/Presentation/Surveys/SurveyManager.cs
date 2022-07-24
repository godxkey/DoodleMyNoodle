using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SurveyManager : GameSystem<SurveyManager>
{
    [System.Serializable]
    public struct DefaultSurveyReference
    {
        public GameAction.ParameterDescriptionType ParameterType;
        public GameObject Survey;
    }

    [SerializeField] private Transform _surveyContainer;
    [SerializeField] private List<DefaultSurveyReference> _defaultSurveys;
    [SerializeField] private SurveyBaseController _nothingSurvey;

    private SurveyBaseController _currentSurvey;
    private Transform _surveyTargetLocation;

    public bool IsSurveyRunning => _currentSurvey != null && _currentSurvey.Running;

    public void BeginSurvey(KeyCode pressedKey,
        Transform surveyLocation,
        GameAction.ExecutionContext useContext,
        List<GameAction.ParameterData> currentResultData,
        GameAction.ParameterDescription[] parameters,
        SurveyBaseController surveyPrefab,
        System.Action<List<GameAction.ParameterData>> onCompleteCallback,
        System.Action onCancelCallback)
    {
        surveyPrefab ??= _nothingSurvey;
        _surveyTargetLocation = surveyLocation;
        GameObject surveyInstance = Instantiate(surveyPrefab.gameObject, _surveyContainer);
        var surveyTransform = surveyInstance.transform;
        surveyTransform.localPosition = Vector3.zero;
        surveyTransform.localRotation = Quaternion.identity;
        surveyTransform.localScale = Vector3.zero;
        SurveyBaseController surveyController = surveyInstance.GetComponent<SurveyBaseController>();

        if (surveyController != null)
        {
            _currentSurvey = surveyController;

            UpdateSurveyPosition();

            surveyController.StartSurvey(pressedKey, completeCallback: (List<GameAction.ParameterData> resultData) =>
            {
                onCompleteCallback.Invoke(resultData);

                if (surveyController.DelayBeforeDestruction > 0)
                {
                    GameObject surveyObject = surveyController.gameObject;
                    this.DelayedCall(surveyController.DelayBeforeDestruction, () =>
                    {
                        if (surveyObject != null)
                        {
                            Destroy(surveyObject);
                        }
                    });
                }
                else
                {
                    Destroy(surveyController.gameObject);
                }

            }, onCancelCallback, useContext, currentResultData, parameters);
        }
        else
        {
            Debug.LogError("Skipping request because it couldn't start");

            // can't start mini-game, complete it immediatly
            onCompleteCallback.Invoke(new List<GameAction.ParameterData>());
        }
    }

    public void BeginDefaultSurvey(KeyCode pressedKey, Transform requestLocation, GameAction.ExecutionContext useContext, List<GameAction.ParameterData> currentResultData, GameAction.ParameterDescription parameterDescription, System.Action<List<GameAction.ParameterData>> onCompleteCallback, System.Action onCancelCallback)
    {
        SurveyBaseController surveyPrefab = null;

        // find default survey for param
        foreach (DefaultSurveyReference request in _defaultSurveys)
        {
            if (request.ParameterType == parameterDescription.GetParameterDescriptionType())
            {
                surveyPrefab = request.Survey.GetComponent<SurveyBaseController>();
                break;
            }
        }

        if (surveyPrefab != null)
        {
            BeginSurvey(pressedKey, requestLocation, useContext, currentResultData, new GameAction.ParameterDescription[] { parameterDescription }, surveyPrefab, onCompleteCallback, onCancelCallback);
        }
    }

    public void StopCurrentSurvey()
    {
        if (_currentSurvey != null)
        {
            _currentSurvey.Cancel();

            if (_currentSurvey.DelayBeforeDestruction > 0)
            {
                GameObject surveyObject = _currentSurvey.gameObject;
                this.DelayedCall(_currentSurvey.DelayBeforeDestruction, () =>
                {
                    if (surveyObject != null)
                    {
                        Destroy(surveyObject);
                    }
                });
            }
            else
            {
                Destroy(_currentSurvey.gameObject);
            }
        }
    }

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        UpdateSurveyPosition();
    }

    private void UpdateSurveyPosition()
    {
        if (_currentSurvey != null && _surveyTargetLocation != null)
        {
            _surveyContainer.position = _surveyTargetLocation.position;
        }
    }
}