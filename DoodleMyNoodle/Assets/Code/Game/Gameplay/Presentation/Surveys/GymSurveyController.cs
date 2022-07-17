using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class GymSurveyController : MonoBehaviour
{
    private const string DROPDOWN_INDEX_SAVE_KEY = "gym-survey-index";

    [SerializeField] private Transform _focusLocation;
    [SerializeField] private TMP_Dropdown _surveyDropdown;
    [SerializeField] private Button _restartButton;
    
    private List<SurveyBaseController> _availableSuveys;
    private SurveyBaseController _requestedSurveyPrefab;

    private const string SURVEY_PREFABS_PATH = "Assets/GameContent/Presentation/Surveys";

    private void Awake()
    {
        _availableSuveys = new List<SurveyBaseController>();

#if UNITY_EDITOR
        var assetPaths = UnityEditor.AssetDatabase.FindAssets("t:prefab", new string[] { SURVEY_PREFABS_PATH });
        for (int i = 0; i < assetPaths.Length; i++)
        {
            var go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(UnityEditor.AssetDatabase.GUIDToAssetPath(assetPaths[i]));
            if(go && go.TryGetComponent(out SurveyBaseController survey))
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
            StartNewSurvey(KeyCode.R, _requestedSurveyPrefab);
            _requestedSurveyPrefab = null;
        }
    }

    private void StartNewSurvey(KeyCode pressedKey, SurveyBaseController surveyPrefab)
    {
        if (SurveyManager.Instance.IsSurveyRunning)
        {
            SurveyManager.Instance.StopCurrentSurvey();
        }

        if (surveyPrefab != null)
        {
            var useContext = new GameAction.ExecutionContext();

            UIStateMachine.Instance.TransitionTo(UIStateType.ParameterSelection);

            SurveyManager.Instance.BeginSurvey(pressedKey, _focusLocation, useContext, new List<GameAction.ParameterData>(), surveyPrefab.CreateDebugQuery(), surveyPrefab, this.OnSurveyComplete, this.OnSurveyCancel);
        }
    }

    private void OnSurveyCancel()
    {
        Log.Info("---------------------- Survey Cancelled ----------------------");
        Log.Info("-----------------------------------------------------------");

        UIStateMachine.Instance.TransitionTo(UIStateType.Gameplay);
    }

    private void OnSurveyComplete(List<GameAction.ParameterData> obj)
    {
        Log.Info("---------------------- Survey Result ----------------------");
        foreach (var item in obj)
        {
            Log.Info($"{item.GetType().GetPrettyName()}: {item}");
        }
        Log.Info("-----------------------------------------------------------");

        UIStateMachine.Instance.TransitionTo(UIStateType.Gameplay);
    }
}
