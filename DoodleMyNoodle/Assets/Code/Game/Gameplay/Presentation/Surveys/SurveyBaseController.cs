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
        public Action.UseContext UseContext;
        public Action.ParameterDescription[] QueryParams;
        public List<Action.ParameterData> CurrentData;

        public Entity ActionPrefab => UseContext.ActionPrefab;
        public Entity Instigator => UseContext.ActionInstigator;

        public T GetQueryParam<T>() where T : Action.ParameterDescription
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
    public float DelayBeforeDestruction = 0;

    public GamePresentationCache Cache => GamePresentationCache.Instance;
    public ISimWorldReadAccessor SimWorld => Cache.SimWorld;

    public bool Running { get; private set; }
    public Context CurrentContext { get; private set; }
    public Action.ParameterDescriptionType[] ExpectedQuery
    {
        get
        {
            if (_cachedExpectedQuery == null)
                _cachedExpectedQuery = GetExpectedQuery();
            return _cachedExpectedQuery;
        }
    }

    private List<Action.ParameterData> _result = new List<Action.ParameterData>();
    private System.Action<List<Action.ParameterData>> _onCompleteCallback;
    private System.Action _cancelCallback;
    private Coroutine _currentLoop;
    [NonSerialized]
    private Action.ParameterDescriptionType[] _cachedExpectedQuery = null;

    public void StartSurvey(System.Action<List<Action.ParameterData>> completeCallback, System.Action cancelCallback, Action.UseContext useContext, List<Action.ParameterData> currentResultData, params Action.ParameterDescription[] parameters)
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

        ShowCostPreview();

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
        HideCostPreview();

        OnEndSurvey(completed);

        if (_currentLoop != null)
            StopCoroutine(_currentLoop);

        OnComplete(completed);
    }

    private void OnComplete(bool completed)
    {
        if (completed)
        {
            _onCompleteCallback?.Invoke(_result);
        }
        else
        {
            _cancelCallback?.Invoke();
        }
    }
    
    protected abstract Action.ParameterDescriptionType[] GetExpectedQuery();
    protected abstract IEnumerator SurveyRoutine(Context context, List<Action.ParameterData> currentData, System.Action complete, System.Action cancel);
    protected abstract void OnEndSurvey(bool wasCompleted);
    public virtual Action.ParameterDescription[] CreateDebugQuery() => new Action.ParameterDescription[0] { };

    private void ShowCostPreview()
    {
        if (SimWorld.TryGetComponent(CurrentContext.Instigator, out ActionPoints ap))
        {
            if (SimWorld.TryGetComponent(CurrentContext.ActionPrefab, out ActionSettingAPCost apCost))
            {
                APEnergyBarDisplayManagementSystem.Instance.ShowCostPreview(CurrentContext.Instigator, (float)ap.Value - apCost.Value);
            } 
        }
    }

    private void HideCostPreview()
    {
        if (SimWorld.HasComponent<ActionPoints>(CurrentContext.Instigator))
        {
            APEnergyBarDisplayManagementSystem.Instance.HideCostPreview(CurrentContext.Instigator);
        }
    }
}