using System;
using System.Collections;
using System.Collections.Generic;
using static Unity.Mathematics.math;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using Unity.Collections;

public class TileHighlightManager : GamePresentationSystem<TileHighlightManager>
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
    private void CreateTile()
    {
        GameObject newHighlight = Instantiate(HighlightPrefab, transform);
        _highlights.Add(newHighlight);

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
        _currentTileSelectionCallback = null;
        SimWorld.TryGetComponentData(PlayerHelpers.GetLocalSimPawnEntity(SimWorld), out FixTranslation localPawnPosition);
        AddHilightsAroundPlayer(roundToInt(localPawnPosition.Value).xy, TileParameters.RangeFromInstigator, TileParameters.Filter, TileParameters.IncludeSelf);
        _currentTileSelectionCallback = TileSelectedData;
    }

    public void InterruptTileSelectionProcess()
    {
        _currentTileSelectionCallback = null;
        HideAll();
    }

    private void AddHilightsAroundPlayer(int2 pos, int depth, TileFilterFlags tileFlags, bool includeSelf)
    {
        int numberOfTiles = 0;
        fix2 newPossibleDestination = new fix2(pos.x, pos.y);

        if (includeSelf)
        {
            HandleHighlightOnPosition(newPossibleDestination, newPossibleDestination, ref numberOfTiles, depth, tileFlags);
        }
        
        for (int i = 1; i <= depth; i++)
        {
            for (int j = 1; j <= (i * 4); j++)
            {
                AddHilight(newPossibleDestination, ref numberOfTiles, i, j, depth, tileFlags);
            }
        }
    }

    private void AddHilight(fix2 newPossibleDestination, ref int numberOfTiles, int height, int lenght, int maxPathCost, TileFilterFlags tileFlags)
    {
        fix2 originPos = newPossibleDestination;
        fix2 destinationPos = FindNewPosibleHighlightDestination(newPossibleDestination, height, lenght);
        
        HandleHighlightOnPosition(originPos, destinationPos, ref numberOfTiles, maxPathCost, tileFlags);
    }

    private fix2 FindNewPosibleHighlightDestination(fix2 newPossibleDestination, int height, int lenght)
    {
        int currentQuadran = Mathf.CeilToInt((float)lenght / height);

        int displacementForward = ((currentQuadran * height + 1) - lenght);
        int displacementSide = (lenght - (((currentQuadran - 1) * height) + 1));

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

        return newPossibleDestination;
    }

    private void HandleHighlightOnPosition(fix2 originPos, fix2 destinationPos, ref int numberOfTiles, int maxPathCost, TileFilterFlags tileFlags)
    {
        int2 tilePosition = roundToInt(destinationPos).xy;

        // TILE FILTERS

        // tile valid
        Entity currentTile = CommonReads.GetTileEntity(SimWorld, tilePosition);
        if (currentTile == Entity.Null)
        {
            return;
        }

        // tile filters
        if (!CommonReads.DoesTileRespectFilters(SimWorld, currentTile, tileFlags))
        {
            return;
        }

        // tile reachable
        if ((tileFlags & TileFilterFlags.Navigable) != 0)
        {
            NativeList<int2> _highlightNavigablePath = new NativeList<int2>(Allocator.Temp);
            if (!CommonReads.FindNavigablePath(SimWorld, roundToInt(originPos).xy, tilePosition, maxPathCost, _highlightNavigablePath))
            {
                return;
            }
        }

        // ADDING MISSING TILE

        while (_highlights.Count - numberOfTiles <= 0)
        {
            // We dont have enough, need to spawn a new one
            CreateTile();
        }

        // ACTIVATE TILE

        _highlights[numberOfTiles].SetActive(true);
        _highlights[numberOfTiles].transform.position = new Vector3((float)destinationPos.x, (float)destinationPos.y, 0);

        numberOfTiles++;
    }
}
