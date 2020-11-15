using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class TileHighlightSurveyController : SurveyBaseController
{
    private GameAction.ParameterData _tileSelected;

    protected override List<GameAction.ParameterData> GetResult()
    {
        List<GameAction.ParameterData> results = new List<GameAction.ParameterData>();
        if (_tileSelected != null)
        {
            results.Add(_tileSelected);
        }
        
        return results;
    }

    protected override string GetTextResult()
    {
        return "Tiles Found";
    }

    protected override IEnumerator SurveyLoop()
    {
        yield return null;
    }

    protected override void OnStartSurvey() 
    {
        if (_parameters.Length < 1)
        {
            Complete();
            return;
        }

        foreach (GameAction.ParameterDescription parameter in _parameters)
        {
            if (parameter is GameActionParameterTile.Description ParameterTile)
            {
                if (ParameterTile != null)
                {
                    TileHighlightManager.Instance.AskForSingleTileSelectionAroundPlayer(ParameterTile, (GameAction.ParameterData TileSelectedData) =>
                    {
                        _tileSelected = TileSelectedData;
                        Complete();
                    });

                    return; // only one at a time, we'll get back here anyway at next UI state entry if more tile are needed
                }
            }
            else
            {
                Debug.LogError("We have been requested a tilehilight survey for parameters that are not even for tiles");
                Complete();
                return;
            }
        }
        
    }
}
