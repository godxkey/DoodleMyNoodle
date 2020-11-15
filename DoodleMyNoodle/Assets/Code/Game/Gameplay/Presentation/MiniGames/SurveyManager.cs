using System;
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

    private Action<List<GameAction.ParameterData>> _currentSurveyCallback;

    public void RequestData(Vector3 surveyLocation,
        GameObject survey,  
        Action<List<GameAction.ParameterData>> onCompleteCallback, 
        params GameAction.ParameterDescription[] parameters)
    {
        _currentSurveyCallback = onCompleteCallback;

        GameObject SurveyInstance = Instantiate(survey, surveyLocation, Quaternion.identity, _surveyContainer);

        SurveyBaseController surveyController = SurveyInstance.GetComponent<SurveyBaseController>();

        if (surveyController != null)
        {
            surveyController.StartSurvey(delegate(List<GameAction.ParameterData> resultData)
            {
                _currentSurveyCallback.Invoke(resultData);
            }, parameters);
        }
        else
        {
            Debug.LogError("Skipping request because it couldn't start");

            // can't start mini-game, complete it immediatly
            List<GameAction.ParameterData> DefaultResults = new List<GameAction.ParameterData>();
            _currentSurveyCallback.Invoke(DefaultResults);
        }
    }

    public void RequestDataWithDefaultSurvey(Vector3 requestLocation, GameAction.ParameterDescription parameterDescription, Action<List<GameAction.ParameterData>> onCompleteCallback)
    {
        foreach (DefaultSurveyReference request in _defaultSurveys)
        {
            if (request.ParameterType == parameterDescription.GetParameterDescriptionType())
            {
                RequestData(requestLocation, request.Survey, onCompleteCallback, parameterDescription);
                return;
            }
        }
    }
}