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

    [NonSerialized] protected bool _isComplete = true; // must be true on start
    [NonSerialized] private bool _hasStarted = false;
    [NonSerialized] protected GameAction.ParameterDescription[] _parameters;

    private Action<List<GameAction.ParameterData>> _onCompleteCallback;
    private Coroutine _currentLoop;

    public void StartSurvey(Action<List<GameAction.ParameterData>> callback, params GameAction.ParameterDescription[] parameters)
    {
        _isComplete = false;
        _onCompleteCallback = callback;
        _hasStarted = true;
        _parameters = parameters;

        if (DebugDisplay != null)
        {
            DebugDisplay.gameObject.SetActive(false);
        }

        OnStartSurvey();

        _currentLoop = StartCoroutine(SurveyLoop());
    }

    protected virtual void OnStartSurvey() { }

    public virtual void Complete()
    {
        if (_isComplete)
            return;

        _isComplete = true;

        if (_currentLoop != null)
            StopCoroutine(_currentLoop);

        if (_onCompleteCallback != null)
        {
            _onCompleteCallback.Invoke(GetResult());
        }

        if (DebugMode)
        {
            string debugText = GetDebugResult();
            if (DebugDisplay != null)
            {
                DebugDisplay.text = debugText;
                DebugDisplay.gameObject.SetActive(true);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected abstract IEnumerator SurveyLoop();

    private void Update()
    {
        if (!_hasStarted && DebugMode && InitOnStart)
        {
            StartSurvey(null);
        }

        OnUpdate();
    }

    protected virtual void OnUpdate() { }
    protected abstract List<GameAction.ParameterData> GetResult();
    protected abstract string GetDebugResult();
}
