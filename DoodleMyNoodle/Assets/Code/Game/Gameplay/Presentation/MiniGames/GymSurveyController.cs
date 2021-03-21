using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class GymSurveyController : MonoBehaviour
{
    [SerializeField] private Vector2 _focusLocation;
    [SerializeField] private TMP_Dropdown _surveyDropdown;
    [SerializeField] private Button _restartButton;
    [SerializeField] private SurveyBaseController2[] _availableSuveys;

    private SurveyBaseController2 _requestedSurveyPrefab;

    private void Awake()
    {
        _surveyDropdown.ClearOptions();
        _surveyDropdown.AddOptions(_availableSuveys.Select((survey) => survey.gameObject.name).ToList());
        _surveyDropdown.value = 0;
        _surveyDropdown.onValueChanged.AddListener((x) => RequestSurvey());
        _restartButton.onClick.AddListener(RequestSurvey);
    }

    private void RequestSurvey()
    {
        // we queue this in the update instead of doing it right away to make sure we alway begin it AFTER clicking the Ui
        _requestedSurveyPrefab = _availableSuveys[_surveyDropdown.value];
    }

    private void Update()
    {
        if (!Game.Started)
            return;

        if (Input.GetKeyDown(KeyCode.R))
            RequestSurvey();

        if (_requestedSurveyPrefab != null)
        {
            StartNewSurvey(_requestedSurveyPrefab);
            _requestedSurveyPrefab = null;
        }
    }

    private void StartNewSurvey(SurveyBaseController2 surveyPrefab)
    {
        if (SurveyManager.Instance.IsSurveyRunning)
        {
            SurveyManager.Instance.StopCurrentSurvey();
        }


        if (surveyPrefab != null)
        {
            SurveyManager.Instance.BeginSurvey(_focusLocation, surveyPrefab, OnSurveyComplete, null);
        }
    }

    private void OnSurveyComplete(List<GameAction.ParameterData> obj)
    {
        Log.Info("---------------------- Survey Result ----------------------");
        foreach (var item in obj)
        {
            Log.Info($"{item.GetType().GetPrettyName()}: {item}");
        }
        Log.Info("-----------------------------------------------------------");
    }
}
