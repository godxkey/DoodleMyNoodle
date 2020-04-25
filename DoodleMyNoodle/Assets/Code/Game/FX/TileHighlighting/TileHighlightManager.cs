using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlightManager : GameSystem<TileHighlightManager>
{
    public GameObject HighlightPrefab;

    List<GameObject> _highlights = new List<GameObject>();

    public override bool SystemReady => true;

    public override void OnSafeDestroy()
    {
        base.OnSafeDestroy();

        for (int i = 0; i < _highlights.Count; i++)
        {
            _highlights[i].Destroy();
        }
        _highlights.Clear();
    }

    public void AskForSingleTileSelectionAroundPlayer(GameActionParameterTile.Description TileParameters, Action<GameActionParameterTile.Data> TileSelectedData)
    {
        AddHilightsAroundPlayer(TileParameters.InstigatorTilePosition, TileParameters.RangeFromInstigator, TileParameters.Filter);
        // remember this action is the one to do when highlight is selected
    }

    private void AddHilightsAroundPlayer(fix3 pos, int depth, TileFilterFlags ignoredTileFlags)
    {
        for (int i = 1; i <= depth; i++)
        {
            for (int j = 1; j <= (i * 4); j++)
            {
                fix2 newPossibleDestination = new fix2(pos.x, pos.y);

                int currentTotalTiles = i * 4;

                int currentQuadran = Mathf.CeilToInt((float)j / i);

                int displacementForward = ((currentQuadran * i + 1) - j);
                int displacementSide = (j - (((currentQuadran - 1) * i) + 1));

                // 4 Quadran
                switch (currentQuadran)
                {
                    case 1:
                        newPossibleDestination.x += -1 * displacementForward;
                        newPossibleDestination.y += displacementSide;
                        break;
                    case 2:
                        newPossibleDestination.x += displacementSide;
                        newPossibleDestination.y += displacementForward;
                        break;
                    case 3:
                        newPossibleDestination.x += displacementForward;
                        newPossibleDestination.y += -1 * displacementSide;
                        break;
                    case 4:
                        newPossibleDestination.x += -1 * displacementSide;
                        newPossibleDestination.y += -1 * displacementForward;
                        break;
                    default:
                        break;
                }

                NeedNewTileAtPosition(new fix2(newPossibleDestination.x, newPossibleDestination.y));
            }
        }
    }

    private void NeedNewTileAtPosition(fix2 position)
    {
        GameObject newHighlight = HighlightPrefab.Duplicate();
        _highlights.Add(newHighlight);

        newHighlight.SetActive(true);
        newHighlight.transform.position = new Vector3((float)position.x, (float)position.y,0);

        newHighlight.GetComponent<HighlightClicker>()?.OnClicked.AddListener(OnHighlightClicked);
    }

    private void OnHighlightClicked(Vector2 tileHighlightClicked)
    {
        // trigger current action to be done when highlight is clicked, see AskForTileSelection
    }

    private void HideAll()
    {
        for (int i = 0; i < _highlights.Count; i++)
        {
            _highlights[i].SetActive(false);
        }
    }
}
