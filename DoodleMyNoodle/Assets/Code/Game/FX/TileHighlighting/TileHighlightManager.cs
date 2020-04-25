using System;
using System.Collections;
using System.Collections.Generic;
using static Unity.Mathematics.math;
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

    private void OnHighlightClicked(Vector2 tileHighlightClicked)
    {
        if (_currentTileSelectionCallback != null)
        {
            GameActionParameterTile.Data TileSelectionData = new GameActionParameterTile.Data(0, int2((int)tileHighlightClicked.x, (int)tileHighlightClicked.y));
            _currentTileSelectionCallback?.Invoke(TileSelectionData);
            HideAll();
        }
    }

    private void OnNewHighlightPromptRequestStarted()
    {
        _currentTileSelectionCallback = null;
    }

    private void NeedNewTileAtPosition(fix2 position)
    {
        GameObject newHighlight = Instantiate(HighlightPrefab, transform);
        _highlights.Add(newHighlight);

        newHighlight.SetActive(true);
        newHighlight.transform.position = new Vector3((float)position.x, (float)position.y, 0);

        newHighlight.GetComponent<HighlightClicker>().OnClicked = OnHighlightClicked;
    }

    private void HideAll()
    {
        for (int i = 0; i < _highlights.Count; i++)
        {
            _highlights[i].SetActive(false);
        }
    }

    // TILE PROMPT

    private Action<GameActionParameterTile.Data> _currentTileSelectionCallback;
    public void AskForSingleTileSelectionAroundPlayer(GameActionParameterTile.Description TileParameters, Action<GameActionParameterTile.Data> TileSelectedData)
    {
        OnNewHighlightPromptRequestStarted();
        GameMonoBehaviourHelpers.SimulationWorld.TryGetComponentData<FixTranslation>(PlayerHelpers.GetLocalSimPawnEntity(GameMonoBehaviourHelpers.SimulationWorld), out FixTranslation localPawnPosition);
        AddHilightsAroundPlayer(localPawnPosition.Value, TileParameters.RangeFromInstigator, TileParameters.Filter);
        _currentTileSelectionCallback = TileSelectedData;
    }

    private void AddHilightsAroundPlayer(fix3 pos, int depth, TileFilterFlags ignoredTileFlags)
    {
        int numberOfTiles = 0;
        for (int i = 1; i <= depth; i++)
        {
            for (int j = 1; j <= (i * 4); j++)
            {
                fix2 newPossibleDestination = new fix2(pos.x, pos.y);

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

                // TODO : Check filters of tile

                // We need a tile highlight
                if(_highlights.Count - numberOfTiles <= 0)
                {
                    // We dont have enough, need to spawn one
                    NeedNewTileAtPosition(new fix2(newPossibleDestination.x, newPossibleDestination.y));
                }
                else
                {
                    // re-use that was already spawned
                    _highlights[numberOfTiles].SetActive(true);
                    _highlights[numberOfTiles].transform.position = new Vector3((float)newPossibleDestination.x, (float)newPossibleDestination.y, 0);
                }

                numberOfTiles++;
            }
        }
    }
}
