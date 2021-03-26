using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurveyClickTiming : SurveyBaseController, IWorldUIPointerClickHandler
{
    public TextMeshPro NumberDisplay;
    public float TimeBetweenChanges = 0.5f;
    [SerializeField] private float _clickPunchDuration;
    [SerializeField] private float _clickPunchScale = 1.35f;
    [SerializeField] private int _maxNumber = 5;
    [SerializeField] private float _finishPauseDuration = 1;

    private bool _clicked;
    private Tweener _clickAnim;
    private DirtyValue<int> _currentNumber;
    private float _changeTimer = 0;

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.SuccessRating,
    };

    protected override IEnumerator SurveyRoutine(GameAction.ParameterDescription[] queryParams, List<GameAction.ParameterData> result, Action complete, Action cancel)
    {
        _currentNumber.Set(0);

        while (!_clicked)
        {
            yield return null;
        }

        // wait for punch anim to be done
        while (_clickAnim != null && _clickAnim.IsPlaying())
        {
            yield return null;
        }

        yield return new WaitForSeconds(_finishPauseDuration);

        result.Add(new GameActionParameterSuccessRate.Data((SurveySuccessRating)_currentNumber.Get()));
        complete();
    }

    void Update()
    {
        if (!_clicked)
        {
            if (_changeTimer < 0)
            {
                _changeTimer += TimeBetweenChanges;
                _currentNumber.Set(_currentNumber + 1);

                if (_currentNumber > _maxNumber)
                {
                    _currentNumber.Set(1);
                }
            }
            _changeTimer -= Time.deltaTime;
        }

        if (_currentNumber.ClearDirty())
        {
            NumberDisplay.text = _currentNumber.Get().ToString();
        }
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
        _clicked = false;
        transform.localScale = Vector3.one;
    }

    void IWorldUIPointerClickHandler.OnPointerClick()
    {
        _clicked = true;
        _clickAnim = transform.DOPunchScale(Vector3.one * _clickPunchScale, _clickPunchDuration, 2, 1);
    }
}
