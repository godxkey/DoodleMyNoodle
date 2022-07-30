using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyNothing : SurveyBaseController
{
    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[0];

    protected override IEnumerator SurveyRoutine(Context context, List<GameAction.ParameterData> result, System.Action complete, System.Action cancel)
    {
        yield return null;
        while (!ShouldStop())
        {
            yield return null;
        }
        bool ShouldStop() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(context.PressedKey) || Input.GetKeyUp(context.PressedKey);

        complete();
        yield break;
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
    }
}
