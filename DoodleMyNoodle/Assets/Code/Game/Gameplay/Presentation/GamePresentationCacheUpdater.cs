using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class GamePresentationCache
{
    public readonly static GamePresentationCache Instance = new GamePresentationCache();

    public bool Ready;

    public Entity LocalPawn;
    public fix3 LocalPawnPosition;
    public int2 LocalPawnTile;
    public Entity LocalPawnTileEntity;
    public Vector3 LocalPawnPositionFloat;
    public Entity LocalController;
    public Team LocalPawnTeam;
    public Team CurrentTeam;
    public TileWorld TileWorld;
    public int2 TileUnderCursor;
    public List<Entity> TileActorsUnderCursor = new List<Entity>();
    public ExternalSimWorldAccessor SimWorld;
}

// should we change this to a component system ?
[AlwaysUpdateSystem]
public class GamePresentationCacheUpdater : ViewComponentSystem
{
    GamePresentationCache Cache => GamePresentationCache.Instance;

    protected override void OnCreate()
    {
        base.OnCreate();

        SimWorldAccessor.OnEntityClearedAndReplaced += ResetCache;

        ResetCache();
        Cache.Ready = true;
        Cache.SimWorld = SimWorldAccessor;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        SimWorldAccessor.OnEntityClearedAndReplaced -= ResetCache;

        ResetCache();
        Cache.Ready = false;
        Cache.SimWorld = null;
    }

    private void ResetCache()
    {
        Cache.LocalController = Entity.Null;
        Cache.LocalPawn = Entity.Null;
        Cache.LocalPawnTileEntity = Entity.Null;
        Cache.TileWorld = default;
        Cache.TileActorsUnderCursor.Clear();
    }

    protected override void OnUpdate()
    {
        ////////////////////////////////////////////////////////////////////////////////////////
        //      Tile World
        ////////////////////////////////////////////////////////////////////////////////////////
        CreateTileWorldSystem createTileWorldSystem = Cache.SimWorld.GetExistingSystem<CreateTileWorldSystem>();
        if (createTileWorldSystem.HasSingleton<GridInfo>())
        {
            Cache.TileWorld = createTileWorldSystem.GetTileWorld();
        }
        else
        {
            Cache.TileWorld = default;
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Current Team
        ////////////////////////////////////////////////////////////////////////////////////////
        Cache.CurrentTeam = new Team() { Value = Cache.SimWorld.GetSingleton<TurnCurrentTeamSingletonComponent>().Value };


        ////////////////////////////////////////////////////////////////////////////////////////
        //      Player & Pawn
        ////////////////////////////////////////////////////////////////////////////////////////
        Cache.LocalPawn = PlayerHelpers.GetLocalSimPawnEntity(Cache.SimWorld);

        if (Cache.LocalPawn != Entity.Null)
        {
            Cache.LocalController = CommonReads.GetPawnController(Cache.SimWorld, Cache.LocalPawn);
            if (Cache.LocalController != Entity.Null && Cache.SimWorld.TryGetComponentData(Cache.LocalController, out Team pawnTeam))
            {
                Cache.LocalPawnTeam = pawnTeam;
            }

            Cache.LocalPawnPosition = Cache.SimWorld.GetComponentData<FixTranslation>(Cache.LocalPawn).Value;
            Cache.LocalPawnPositionFloat = Cache.LocalPawnPosition.ToUnityVec();
            Cache.LocalPawnTile = Helpers.GetTile(Cache.LocalPawnPosition);
            Cache.LocalPawnTileEntity = Cache.TileWorld.GetEntity(Cache.LocalPawnTile);
        }
        else
        {
            Cache.LocalController = Entity.Null;
            Cache.LocalPawnTileEntity = Entity.Null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Tile Actors Under Cursor
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            Vector3 mouseWorldPos = CameraService.Instance.ActiveCamera.ScreenToWorldPoint(Input.mousePosition);
            Cache.TileUnderCursor = Helpers.GetTile(mouseWorldPos);

            Cache.TileActorsUnderCursor.Clear();
            Entity tileEntity = Cache.TileWorld.GetEntity(Cache.TileUnderCursor);
            if (tileEntity != Entity.Null)
            {
                foreach (var tileActor in Cache.SimWorld.GetBufferReadOnly<TileActorReference>(tileEntity))
                {
                    Cache.TileActorsUnderCursor.Add(tileActor);
                }
            }
        }
    }
}
