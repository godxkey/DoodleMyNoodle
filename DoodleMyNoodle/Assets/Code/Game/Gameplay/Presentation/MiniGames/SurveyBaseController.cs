using CCC.InspectorDisplay;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class SurveyBaseController : MonoBehaviour
{
    public bool DebugMode = false;
    [ShowIf("DebugMode")]
    public TextMeshPro DebugDisplay;

    public string DisplayName;

    protected bool _isComplete = false;

    private Action<List<GameAction.ParameterData>> _onCompleteCallback;
    private bool _hasStarted = false;
    private Coroutine _currentLoop;

    protected GameAction.ParameterDescription[] _parameters;

    public virtual void StartSurvey(Action<List<GameAction.ParameterData>> callback, params GameAction.ParameterDescription[] parameters)
    {
        _isComplete = false;
        _onCompleteCallback = callback;
        _hasStarted = true;
        _parameters = parameters;

        if (DebugDisplay != null)
        {
            DebugDisplay.gameObject.SetActive(false);
        }

        _currentLoop = StartCoroutine(SurveyLoop());

        OnStartSurvey();
    }

    protected virtual void OnStartSurvey() { }

    public virtual void Complete()
    {
        if (!_isComplete)
        {
            _isComplete = true;
            StopCoroutine(_currentLoop);

            if (_onCompleteCallback != null)
            {
                _onCompleteCallback.Invoke(GetResult());
            }

            if (DebugMode)
            {
                DebugDisplay.text = GetTextResult();
                DebugDisplay.gameObject.SetActive(true);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    protected abstract IEnumerator SurveyLoop();

    private void Update()
    {
        if (!_hasStarted && DebugMode)
        {
            StartSurvey(null);
        }

        if (_hasStarted)
        {
            OnUpdate();
        }
    }

    protected virtual void OnUpdate() { }
    protected abstract List<GameAction.ParameterData> GetResult();
    protected abstract string GetTextResult();
}
