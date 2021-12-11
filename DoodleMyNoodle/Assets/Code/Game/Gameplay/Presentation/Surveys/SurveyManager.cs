﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SurveyManager : GamePresentationSystem<SurveyManager>
{
    [System.Serializable]
    public struct DefaultSurveyReference
    {
        public GameAction.ParameterDescriptionType ParameterType;
        public GameObject Survey;
    }

    [SerializeField] private Transform _surveyContainer;
    [SerializeField] private List<DefaultSurveyReference> _defaultSurveys;


    SurveyBaseController _currentSurvey;

    public bool IsSurveyRunning => _currentSurvey != null && _currentSurvey.Running;

    public void BeginSurvey(Vector3 surveyLocation,
        GameAction.UseContext useContext,
        List<GameAction.ParameterData> currentResultData,
        GameAction.ParameterDescription[] parameters,
        SurveyBaseController surveyPrefab,
        Action<List<GameAction.ParameterData>> onCompleteCallback,
        Action onCancelCallback)
    {
        GameObject surveyInstance = Instantiate(surveyPrefab.gameObject, surveyLocation, Quaternion.identity, _surveyContainer);

        SurveyBaseController surveyController = surveyInstance.GetComponent<SurveyBaseController>();

        if (surveyController != null)
        {
            _currentSurvey = surveyController;

            surveyController.StartSurvey(delegate (List<GameAction.ParameterData> resultData)
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
            List<GameAction.ParameterData> DefaultResults = new List<GameAction.ParameterData>();
            onCompleteCallback.Invoke(DefaultResults);
        }
    }

    public void BeginDefaultSurvey(Vector3 requestLocation, GameAction.UseContext useContext, List<GameAction.ParameterData> currentResultData, GameAction.ParameterDescription parameterDescription, Action<List<GameAction.ParameterData>> onCompleteCallback, Action onCancelCallback)
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
            BeginSurvey(requestLocation, useContext, currentResultData, new GameAction.ParameterDescription[] { parameterDescription }, surveyPrefab, onCompleteCallback, onCancelCallback);
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
}