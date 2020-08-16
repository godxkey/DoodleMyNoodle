using System;
using System.Collections;
using System.Collections.Generic;
using static Unity.Mathematics.math;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using Unity.Collections;
using UnityEngine.Serialization;

public class TileHighlightManager : GamePresentationSystem<TileHighlightManager>
{
    [FormerlySerializedAs("HighlightPrefab")]
    [SerializeField] private GameObject _highlightPrefab;

    private List<GameObject> _highlights = new List<GameObject>();
    private Action<GameActionParameterTile.Data> _currentTileSelectionCallback;

    public override bool SystemReady => true;

    protected override void OnGamePresentationUpdate() { }

    protected override void OnDestroy()
    {
        base.OnDestroy();

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
            int2 tile = int2(Mathf.RoundToInt(tileHighlightClicked.x), Mathf.RoundToInt(tileHighlightClicked.y));

            GameActionParameterTile.Data TileSelectionData = new GameActionParameterTile.Data(0, tile);

            _currentTileSelectionCallback?.Invoke(TileSelectionData);
            _currentTileSelectionCallback = null;

            HideAll();
        }
    }
    private void CreateHighlight()
    {
        GameObject newHighlight = Instantiate(_highlightPrefab, transform);
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

    public void AskForSingleTileSelectionAroundPlayer(GameActionParameterTile.Description description, Action<GameActionParameterTile.Data> onSelectCallback)
    {
        _currentTileSelectionCallback = onSelectCallback;

        TileFinder tileFinder = new TileFinder(SimWorld);

        TileFinder.Context context = new TileFinder.Context()
        {
            PawnTile = SimWorldCache.LocalPawnTile,
            PawnEntity = SimWorldCache.LocalPawn,
        };

        NativeList<int2> tiles = new NativeList<int2>(Allocator.Temp);

        tileFinder.Find(description, context, tiles);

        // Create new highlights (if necessary)
        while (_highlights.Count <= tiles.Length)
        {
            CreateHighlight();
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            // Activate Highlight
            _highlights[i].SetActive(true);
            _highlights[i].transform.position = new Vector3(tiles[i].x, tiles[i].y, 0);
        }

    }

    public void InterruptTileSelectionProcess()
    {
        _currentTileSelectionCallback = null;
        HideAll();
    }

    private struct TileFinder
    {
        public struct Context
        {
            public int2 PawnTile;
            public Entity PawnEntity;
        }

        private ISimWorldReadAccessor _accessor;

        public TileFinder(ISimWorldReadAccessor accessor)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        public void Find(GameActionParameterTile.Description description, Context context, NativeList<int2> result)
        {
            int2 tileMin = context.PawnTile - int2(description.RangeFromInstigator);
            int2 tileMax = context.PawnTile + int2(description.RangeFromInstigator);

            for (int x = tileMin.x; x <= tileMax.x; x++)
            {
                for (int y = tileMin.y; y <= tileMax.y; y++)
                {
                    TestTile(int2(x, y), description, context, result);
                }
            }
        }

        private void TestTile(int2 tilePosition, GameActionParameterTile.Description description, in Context parameters, NativeList<int2> result)
        {
            if (IsTileOk(tilePosition, description, parameters))
            {
                result.Add(tilePosition);
            }
        }

        private bool IsTileOk(int2 tilePosition, GameActionParameterTile.Description description, in Context parameters)
        {
            if (!description.IncludeSelf && parameters.PawnTile.Equals(tilePosition))
            {
                return false;
            }

            // tile entity valid
            Entity tileEntity = CommonReads.GetTileEntity(_accessor, tilePosition);
            if (tileEntity == Entity.Null)
            {
                return false; // invalid entity
            }

            // tile filters
            var tileFlags = _accessor.GetComponentData<TileFlagComponent>(tileEntity);
            if ((tileFlags & description.TileFilter) == 0)
            {
                return false; // tile is filtered out
            }

            // tile requires an attackable target
            if (description.RequiresAttackableEntity)
            {
                bool hasAtLeastOneAttackableActor = false;
                foreach (TileActorReference tileActor in _accessor.GetBufferReadOnly<TileActorReference>(tileEntity))
                {
                    if (_accessor.Exists(tileActor))
                    {
                        if (_accessor.HasComponent<Health>(tileActor) || _accessor.HasComponent<Armor>(tileActor))
                        {
                            hasAtLeastOneAttackableActor = true;
                            break;
                        }
                    }
                }

                if (!hasAtLeastOneAttackableActor)
                {
                    return false;
                }
            }

            // Custom tile actor predicate
            if (description.CustomTileActorPredicate != null)
            {
                bool hasAtLeastOneMatch = false;
                foreach (TileActorReference tileActor in _accessor.GetBufferReadOnly<TileActorReference>(tileEntity))
                {
                    if (_accessor.Exists(tileActor) && description.CustomTileActorPredicate(tileActor, _accessor))
                    {
                        hasAtLeastOneMatch = true;
                        break;
                    }
                }

                if (!hasAtLeastOneMatch)
                {
                    return false;
                }
            }

            // Custom tile predicate
            if (description.CustomTilePredicate != null)
            {
                if(!description.CustomTilePredicate(tilePosition, tileEntity, _accessor))
                {
                    return false;
                }
            }

            // tile reachable
            if (description.MustBeReachable)
            {
                NativeList<int2> _highlightNavigablePath = new NativeList<int2>(Allocator.Temp);
                if (!CommonReads.FindNavigablePath(_accessor, parameters.PawnTile, tilePosition, description.RangeFromInstigator, _highlightNavigablePath))
                {
                    return false; // tile is non-reachable
                }
            }

            return true;
        }
    }
}
