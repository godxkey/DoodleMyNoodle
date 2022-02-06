using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyTile : SurveyBaseController
{
    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.Tile
    };

    protected override IEnumerator SurveyRoutine(Context context, List<GameAction.ParameterData> result, System.Action complete, System.Action cancel)
    {
        var paramTile = context.GetQueryParam<GameActionParameterTile.Description>();

        TileHighlightManager.Instance.AskForSingleTileSelectionAroundPlayer(paramTile, (GameAction.ParameterData selection) =>
        {
            result.Add(selection);
            complete();
        });

        yield break;  // only one at a time, we'll get back here anyway at next UI state entry if more tile are needed
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
        TileHighlightManager.Instance?.InterruptTileSelectionProcess();
    }

    public override GameAction.ParameterDescription[] CreateDebugQuery()
    {
        return new GameAction.ParameterDescription[]
        {
            new GameActionParameterTile.Description(rangeFromInstigator: 5)
        };
    }
}
