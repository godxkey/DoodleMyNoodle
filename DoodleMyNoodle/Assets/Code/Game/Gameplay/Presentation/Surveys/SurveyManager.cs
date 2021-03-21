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


    SurveyBaseController2 _currentSurvey;

    public bool IsSurveyRunning => _currentSurvey != null && _currentSurvey.Running;

    public void BeginSurvey(Vector3 surveyLocation,
        SurveyBaseController2 surveyPrefab,
        Action<List<GameAction.ParameterData>> onCompleteCallback,
        Action onCancelCallback,
        params GameAction.ParameterDescription[] parameters)
    {
        GameObject surveyInstance = Instantiate(surveyPrefab.gameObject, surveyLocation, Quaternion.identity, _surveyContainer);

        SurveyBaseController2 surveyController = surveyInstance.GetComponent<SurveyBaseController2>();

        if (surveyController != null)
        {
            _currentSurvey = surveyController;

            surveyController.StartSurvey(delegate (List<GameAction.ParameterData> resultData)
            {
                onCompleteCallback.Invoke(resultData);
                Destroy(surveyController.gameObject);
            }, onCancelCallback, parameters);
        }
        else
        {
            Debug.LogError("Skipping request because it couldn't start");

            // can't start mini-game, complete it immediatly
            List<GameAction.ParameterData> DefaultResults = new List<GameAction.ParameterData>();
            onCompleteCallback.Invoke(DefaultResults);
        }
    }

    public void BeginDefaultSurvey(Vector3 requestLocation, GameAction.ParameterDescription parameterDescription, Action<List<GameAction.ParameterData>> onCompleteCallback, Action onCancelCallback)
    {
        SurveyBaseController2 surveyPrefab = null;
        
        // find default survey for param
        foreach (DefaultSurveyReference request in _defaultSurveys)
        {
            if (request.ParameterType == parameterDescription.GetParameterDescriptionType())
            {
                surveyPrefab = request.Survey.GetComponent<SurveyBaseController2>();
                break;
            }
        }

        if (surveyPrefab != null)
        {
            BeginSurvey(requestLocation, surveyPrefab, onCompleteCallback, onCancelCallback, parameterDescription);
        }
    }

    public void StopCurrentSurvey()
    {
        if(_currentSurvey != null)
        {
            _currentSurvey.Cancel();
            Destroy(_currentSurvey.gameObject);
        }
    }
}