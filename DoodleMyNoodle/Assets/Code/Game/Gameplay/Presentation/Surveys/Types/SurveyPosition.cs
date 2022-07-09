using CCC.Fix2D;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.MathematicsX;
using UnityEngine;

public class SurveyPosition : SurveyBaseController
{
    [SerializeField] private Color _outOfRangeColor;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Transform _rangeIndicator1;
    [SerializeField] private Transform _rangeIndicator2;
    [SerializeField] private Transform _line;
    [SerializeField] private float _rangeIndicatorScaleDiff = 0.1f;

    private Vector2 _instigatorPosition;

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.Position
    };

    public override GameAction.ParameterDescription[] CreateDebugQuery()
    {
        return new GameAction.ParameterDescription[] { new GameActionParameterPosition.Description() { MaxRangeFromInstigator = 5 } };
    }

    protected override IEnumerator SurveyRoutine(Context context, List<GameAction.ParameterData> result, System.Action complete, System.Action cancel)
    {
        CursorOverlayService.Instance.SetCursorType(CursorOverlayService.CursorType.Target);

        Update(); // force an update

        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        CursorOverlayService.Instance.ResetCursorToDefault();

        result.Add(new GameActionParameterPosition.Data((fix2)Cache.PointerWorldPosition));
        complete();

        yield break;
    }

    private void Update()
    {
        if (SimWorld.TryGetComponent(CurrentContext.Instigator, out FixTranslation position))
        {
            _instigatorPosition = (Vector2)position.Value;
        }

        var paramPos = CurrentContext.GetQueryParam<GameActionParameterPosition.Description>();

        float range = paramPos == null ? float.MaxValue : (float)paramPos.MaxRangeFromInstigator;

        bool inRange = (Cache.PointerWorldPosition - _instigatorPosition).magnitude < range;

        if (inRange)
        {
            CursorOverlayService.Instance.SetCursorColor(_normalColor);
        }
        else
        {
            CursorOverlayService.Instance.SetCursorColor(_outOfRangeColor);
        }

        // drag line
        if (_line != null)
        {
            Vector2 dragVector = _instigatorPosition - Cache.PointerWorldPosition;
            _line.position = (_instigatorPosition + Cache.PointerWorldPosition) / 2;
            _line.localScale = new Vector3(dragVector.magnitude, 1, 1);
            _line.rotation = Quaternion.Euler(0, 0, math.degrees(mathX.angle2d(dragVector)));
        }

        if (range < 1000)
        {
            _rangeIndicator1.gameObject.SetActive(true);
            _rangeIndicator2.gameObject.SetActive(true);
            _rangeIndicator1.localScale = Vector3.one * range * 2f;
            _rangeIndicator2.localScale = _rangeIndicator1.localScale + Vector3.one * _rangeIndicatorScaleDiff * Cache.CameraSize;
        }
        else
        {
            _rangeIndicator1.gameObject.SetActive(false);
            _rangeIndicator2.gameObject.SetActive(false);
        }
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
        CursorOverlayService.Instance.ResetCursorToDefault();
    }
}