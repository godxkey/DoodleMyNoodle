using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyPosition : SurveyBaseController2
{
    [SerializeField] private Transform _pointIndicator;

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.Position
    };

    protected override IEnumerator SurveyRoutine(GameAction.ParameterDescription[] queryParams, List<GameAction.ParameterData> result, Action complete, Action cancel)
    {
        Vector2 mouseWorldPos = Vector2.zero;

        while (!Input.GetMouseButtonDown(0))
        {
            var activeCamera = CameraService.Instance?.ActiveCamera;

            if (activeCamera != null)
            {
                mouseWorldPos = activeCamera.ScreenToWorldPoint(Input.mousePosition);
                _pointIndicator.position = mouseWorldPos;
            }

            yield return null;
        }

        result.Add(new GameActionParameterPosition.Data((fix2)mouseWorldPos));
        complete();

        yield break;
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
    }
}
