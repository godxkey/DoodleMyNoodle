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
    private const string DROPDOWN_INDEX_SAVE_KEY = "gym-survey-index";

    [SerializeField] private Vector2 _focusLocation;
    [SerializeField] private TMP_Dropdown _surveyDropdown;
    [SerializeField] private Button _restartButton;
    
    private List<SurveyBaseController2> _availableSuveys;
    private SurveyBaseController2 _requestedSurveyPrefab;

    private const string SURVEY_PREFABS_PATH = "Assets/Prefabs/Game/Presentation/Surveys";

    private void Awake()
    {
        _availableSuveys = new List<SurveyBaseController2>();

#if UNITY_EDITOR
        var assetPaths = UnityEditor.AssetDatabase.FindAssets("t:prefab", new string[] { SURVEY_PREFABS_PATH });
        for (int i = 0; i < assetPaths.Length; i++)
        {
            var go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(UnityEditor.AssetDatabase.GUIDToAssetPath(assetPaths[i]));
            if(go && go.TryGetComponent(out SurveyBaseController2 survey))
            {
                _availableSuveys.Add(survey);
            }
        }
#endif

        _surveyDropdown.ClearOptions();
        _surveyDropdown.AddOptions(_availableSuveys.Select((survey) => survey.gameObject.name).ToList());
        _surveyDropdown.value = PlayerPrefs.GetInt(DROPDOWN_INDEX_SAVE_KEY, 0);
        _surveyDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        _restartButton.onClick.AddListener(RequestSurvey);
    }

    private void OnDropdownValueChanged(int arg0)
    {
        PlayerPrefs.SetInt(DROPDOWN_INDEX_SAVE_KEY, arg0);
        PlayerPrefs.Save();
        RequestSurvey();
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
            SurveyManager.Instance.BeginSurvey(_focusLocation, surveyPrefab, OnSurveyComplete, OnSurveyCancel, surveyPrefab.CreateDebugQuery());
        }
    }

    private void OnSurveyCancel()
    {
        Log.Info("---------------------- Survey Cancelled ----------------------");
        Log.Info("-----------------------------------------------------------");
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
