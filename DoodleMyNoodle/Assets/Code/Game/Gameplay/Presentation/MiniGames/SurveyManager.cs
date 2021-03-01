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


    SurveyBaseController _currentSurvey;
    private Action<List<GameAction.ParameterData>> _currentSurveyCallback;

    public void BeginSurvey(Vector3 surveyLocation,
        GameObject survey,
        Action<List<GameAction.ParameterData>> onCompleteCallback,
        params GameAction.ParameterDescription[] parameters)
    {
        _currentSurveyCallback = onCompleteCallback;

        GameObject surveyInstance = Instantiate(survey, surveyLocation, Quaternion.identity, _surveyContainer);

        SurveyBaseController surveyController = surveyInstance.GetComponent<SurveyBaseController>();

        if (surveyController != null)
        {
            _currentSurvey = surveyController;

            surveyController.StartSurvey(delegate (List<GameAction.ParameterData> resultData)
            {
                _currentSurveyCallback.Invoke(resultData);
                Destroy(surveyController.gameObject);
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

    public void BeginDefaultSurvey(Vector3 requestLocation, GameAction.ParameterDescription parameterDescription, Action<List<GameAction.ParameterData>> onCompleteCallback)
    {
        GameObject survey = null;
        
        // find default survey for param
        foreach (DefaultSurveyReference request in _defaultSurveys)
        {
            if (request.ParameterType == parameterDescription.GetParameterDescriptionType())
            {
                survey = request.Survey;
                break;
            }
        }

        if (survey != null)
        {
            BeginSurvey(requestLocation, survey, onCompleteCallback, parameterDescription);
        }
    }

    public void StopCurrentSurvey()
    {
        if(_currentSurvey != null)
        {
            _currentSurvey.Stop();
            Destroy(_currentSurvey.gameObject);
        }
    }
}