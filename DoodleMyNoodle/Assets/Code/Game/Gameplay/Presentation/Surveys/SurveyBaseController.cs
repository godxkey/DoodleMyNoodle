using CCC.InspectorDisplay;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngineX;

public abstract class SurveyBaseController : MonoBehaviour
{
    public GamePresentationCache Cache => GamePresentationCache.Instance;

    public bool DebugMode = false;
    [ShowIf("DebugMode")]
    public TextMeshPro DebugDisplay;
    [ShowIf("DebugMode")]
    public bool InitOnStart = false;

    public string DisplayName;

    [NonSerialized] protected bool _running = false;
    [NonSerialized] private bool _hasStarted = false;
    [NonSerialized] protected GameAction.ParameterDescription[] _parameters;

    private Action<List<GameAction.ParameterData>> _onCompleteCallback;
    private Coroutine _currentLoop;

    public void StartSurvey(Action<List<GameAction.ParameterData>> callback, params GameAction.ParameterDescription[] parameters)
    {
        _running = true;
        _onCompleteCallback = callback;
        _hasStarted = true;
        _parameters = parameters;

        if (DebugDisplay != null)
        {
            DebugDisplay.gameObject.SetActive(false);
        }

        _currentLoop = StartCoroutine(SurveyRoutine());
    }

    public void Stop()
    {
        EndSurvey(completed: false);
    }

    public void Complete()
    {
        EndSurvey(completed: true);
    }

    private void EndSurvey(bool completed)
    {
        if (!_running)
            return;

        _running = false;

        OnEndSurvey(completed);

        if (_currentLoop != null)
            StopCoroutine(_currentLoop);

        if (completed)
        {
            _onCompleteCallback?.Invoke(GetResult());

            if (DebugMode)
            {
                string debugText = GetDebugResult();
                if (DebugDisplay != null)
                {
                    DebugDisplay.text = debugText;
                    DebugDisplay.gameObject.SetActive(true);
                }
            }
        }
    }

    private void Update()
    {
        if (!_hasStarted && DebugMode && InitOnStart)
        {
            StartSurvey(null);
        }

        OnUpdate();
    }

    protected virtual void OnUpdate() { }
    protected abstract IEnumerator SurveyRoutine();
    protected abstract List<GameAction.ParameterData> GetResult();
    protected abstract string GetDebugResult();
    protected abstract void OnEndSurvey(bool wasCompleted);
}

public abstract class SurveyBaseController2 : MonoBehaviour
{
    public GamePresentationCache Cache => GamePresentationCache.Instance;

    public bool Running { get; private set; }
    public GameAction.ParameterDescription[] QueryParameters { get; private set; }
    public GameAction.ParameterDescriptionType[] ExpectedQuery
    {
        get
        {
            if (_cachedExpectedQuery == null)
                _cachedExpectedQuery = GetExpectedQuery();
            return _cachedExpectedQuery;
        }
    }

    private List<GameAction.ParameterData> _result = new List<GameAction.ParameterData>();
    private Action<List<GameAction.ParameterData>> _onCompleteCallback;
    private Action _cancelCallback;
    private Coroutine _currentLoop;
    private GameAction.ParameterDescriptionType[] _cachedExpectedQuery;

    public void StartSurvey(Action<List<GameAction.ParameterData>> completeCallback, Action cancelCallback, params GameAction.ParameterDescription[] parameters)
    {
        Running = true;
        QueryParameters = parameters;

        _onCompleteCallback = completeCallback;
        _cancelCallback = cancelCallback;
        _result.Clear();

        _currentLoop = StartCoroutine(SurveyRoutine(parameters, _result, Complete, Cancel));
    }

    public void Cancel()
    {
        EndSurvey(completed: false);
    }

    private void Complete()
    {
        EndSurvey(completed: true);
    }

    private void EndSurvey(bool completed)
    {
        if (!Running)
            return;

        Running = false;

        OnEndSurvey(completed);

        if (_currentLoop != null)
            StopCoroutine(_currentLoop);

        if (completed)
        {
            _onCompleteCallback?.Invoke(_result);
        }
        else
        {
            _cancelCallback?.Invoke();
        }
    }
    
    protected abstract GameAction.ParameterDescriptionType[] GetExpectedQuery();
    protected abstract IEnumerator SurveyRoutine(GameAction.ParameterDescription[] queryParams, List<GameAction.ParameterData> result, Action complete, Action cancel);
    protected abstract void OnEndSurvey(bool wasCompleted);
    public virtual GameAction.ParameterDescription[] CreateDebugQuery() => new GameAction.ParameterDescription[0] { };
}