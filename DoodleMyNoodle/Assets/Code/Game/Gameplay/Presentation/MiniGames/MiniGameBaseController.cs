using CCC.InspectorDisplay;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class MiniGameBaseController : MonoBehaviour
{
    public bool DebugMode = false;
    [ShowIf("DebugMode")]
    public TextMeshPro DebugDisplay;

    public MiniGameDescriptionBase DefaultDescription;

    protected bool _isComplete = false;

    private Action<GameActionParameterMiniGame.Data> _onCompleteCallback;
    private bool _hasStarted = false;
    private Coroutine _currentGameLoop;

    private MiniGameDescriptionBase _description;

    public virtual void StartMiniGame(MiniGameDescriptionBase miniGameDescription, Action<GameActionParameterMiniGame.Data> callback)
    {
        _isComplete = false;
        _onCompleteCallback = callback;
        _hasStarted = true;
        _description = miniGameDescription;

        DebugDisplay.gameObject.SetActive(false);

        _currentGameLoop = StartCoroutine(MiniGameLoop());
    }

    public virtual void Complete()
    {
        if (_isComplete)
        {
            StartMiniGame(DefaultDescription, null);
        }
        else
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
            StartMiniGame(DefaultDescription, null);
        }

        if (_hasStarted)
        {
            OnUpdate();
        }
    }

    protected T GetMiniGameDescription<T>() where T : MiniGameDescriptionBase
    {
        return _description as T;
    }

    protected virtual void OnUpdate() { }
    protected abstract GameActionParameterMiniGame.Data GetResult();
    protected abstract string GetTextResult();
}
