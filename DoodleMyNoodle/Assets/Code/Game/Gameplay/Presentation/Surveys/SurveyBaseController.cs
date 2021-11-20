using CCC.InspectorDisplay;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;

public abstract class SurveyBaseController : MonoBehaviour
{
    public struct Context
    {
        public GameAction.UseContext UseContext;
        public GameAction.ParameterDescription[] QueryParams;
        public List<GameAction.ParameterData> CurrentData;

        public Entity Item => UseContext.Item;
        public Entity Instigator => UseContext.InstigatorPawn;

        public T GetQueryParam<T>() where T : GameAction.ParameterDescription
        {
            for (int i = 0; i < QueryParams.Length; i++)
            {
                if (QueryParams[i] is T x)
                    return x;
            }
            return null;
        }
    }

    [SerializeField] private TextData _tooltip;

    public GamePresentationCache Cache => GamePresentationCache.Instance;
    public ISimWorldReadAccessor SimWorld => Cache.SimWorld;

    public bool Running { get; private set; }
    public Context CurrentContext { get; private set; }
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
    [NonSerialized]
    private GameAction.ParameterDescriptionType[] _cachedExpectedQuery = null;

    public void StartSurvey(Action<List<GameAction.ParameterData>> completeCallback, Action cancelCallback, GameAction.UseContext useContext, List<GameAction.ParameterData> currentResultData, params GameAction.ParameterDescription[] parameters)
    {
        Running = true;

        InfoTextDisplay.Instance.SetText(_tooltip);

        var context = new Context()
        {
            QueryParams = parameters,
            UseContext = useContext,
            CurrentData = currentResultData
        };

        CurrentContext = context;

        _onCompleteCallback = completeCallback;
        _cancelCallback = cancelCallback;
        _result.Clear();

        _currentLoop = StartCoroutine(SurveyRoutine(context, _result, Complete, Cancel));
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

        InfoTextDisplay.Instance.ForceHideText();

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
    protected abstract IEnumerator SurveyRoutine(Context context, List<GameAction.ParameterData> currentData, Action complete, Action cancel);
    protected abstract void OnEndSurvey(bool wasCompleted);
    public virtual GameAction.ParameterDescription[] CreateDebugQuery() => new GameAction.ParameterDescription[0] { };
}