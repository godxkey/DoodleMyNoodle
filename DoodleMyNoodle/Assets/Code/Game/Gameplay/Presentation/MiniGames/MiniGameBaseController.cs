using CCC.InspectorDisplay;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class GameActionDataRequestBaseController : MonoBehaviour
{
    public bool DebugMode = false;
    [ShowIf("DebugMode")]
    public TextMeshPro DebugDisplay;

    public GameActionRequestDefinitionBase DefaultDefinition;

    protected bool _isComplete = false;

    private Action<List<GameAction.ParameterData>> _onCompleteCallback;
    private bool _hasStarted = false;
    private Coroutine _currentGameLoop;

    private GameActionRequestDefinitionBase _definition;

    public virtual void StartRequest(GameActionRequestDefinitionBase requestDefinition, Action<List<GameAction.ParameterData>> callback)
    {
        _isComplete = false;
        _onCompleteCallback = callback;
        _hasStarted = true;
        _definition = requestDefinition;

        DebugDisplay.gameObject.SetActive(false);

        _currentGameLoop = StartCoroutine(MiniGameLoop());
    }

    public virtual void Complete()
    {
        if (!_isComplete)
        {
            _isComplete = true;
            StopCoroutine(_currentGameLoop);

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

    protected abstract IEnumerator MiniGameLoop();

    private void Update()
    {
        if (!_hasStarted && DebugMode)
        {
            StartRequest(DefaultDefinition, null);
        }

        if (_hasStarted)
        {
            OnUpdate();
        }
    }

    protected T GetMiniGameDefinition<T>() where T : GameActionRequestDefinitionBase
    {
        return _definition as T;
    }

    protected virtual void OnUpdate() { }
    protected abstract List<GameAction.ParameterData> GetResult();
    protected abstract string GetTextResult();
}
