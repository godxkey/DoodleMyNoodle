using SimulationControl;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using CCC.Fix2D;
using UnityEngineX;

public class GamePresentationCache
{
    public readonly static GamePresentationCache Instance = new GamePresentationCache();

    public bool Ready;

    public bool PointerInWorld;
    public Vector2 PointerWorldPosition;
    public int2 PointedTile;
    public List<Entity> PointedTileActors = new List<Entity>();

    public Entity LocalPawn;
    public int LocalPawnHealth;
    public fix2 LocalPawnPosition;
    public int2 LocalPawnTile;
    public Entity LocalPawnTileEntity;
    public Vector2 LocalPawnPositionFloat;
    public Entity LocalController;
    public Team LocalControllerTeam;
    public Team CurrentTeam;
    public TileWorld TileWorld;
    public ExternalSimWorldAccessor SimWorld;

    public bool LocalPawnExists => LocalPawn != Entity.Null;
    public bool LocalControllerExists => LocalController != Entity.Null;
    public bool LocalPawnAlive => LocalPawnExists && LocalPawnHealth > 0;
}

// should we change this to a component system ?
[AlwaysUpdateSystem]
public class GamePresentationCacheUpdater : ViewSystemBase
{
    GamePresentationCache Cache => GamePresentationCache.Instance;

    protected override void OnCreate()
    {
        base.OnCreate();

        SimWorldAccessor.WorldReplaced += ResetCache;

        ResetCache();
        Cache.Ready = true;
        Cache.SimWorld = SimWorldAccessor;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        SimWorldAccessor.WorldReplaced -= ResetCache;

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
        Cache.PointedTileActors.Clear();
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
        if (Cache.SimWorld.TryGetSingleton(out TurnCurrentTeamSingletonComponent curTeam))
        {
            Cache.CurrentTeam = curTeam.Value;
        }
        else
        {
            Cache.CurrentTeam = -1;
        }


        ////////////////////////////////////////////////////////////////////////////////////////
        //      Player & Pawn
        ////////////////////////////////////////////////////////////////////////////////////////
        Cache.LocalPawn = PlayerHelpers.GetLocalSimPawnEntity(Cache.SimWorld);

        if (Cache.LocalPawn != Entity.Null)
        {
            Cache.LocalController = CommonReads.GetPawnController(Cache.SimWorld, Cache.LocalPawn);
            if (Cache.LocalController != Entity.Null && Cache.SimWorld.TryGetComponentData(Cache.LocalController, out Team controllerTeam))
            {
                Cache.LocalControllerTeam = controllerTeam;
            }

            Cache.LocalPawnPosition = Cache.SimWorld.GetComponentData<FixTranslation>(Cache.LocalPawn).Value;
            Cache.LocalPawnHealth = Cache.SimWorld.GetComponentData<Health>(Cache.LocalPawn).Value;
            Cache.LocalPawnPositionFloat = Cache.LocalPawnPosition.ToUnityVec();
            Cache.LocalPawnTile = Helpers.GetTile(Cache.LocalPawnPosition);
            Cache.LocalPawnTileEntity = Cache.TileWorld.IsCreated ? Cache.TileWorld.GetEntity(Cache.LocalPawnTile) : Entity.Null;
        }
        else
        {
            Cache.LocalController = Entity.Null;
            Cache.LocalPawnTileEntity = Entity.Null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Pointer
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            Cache.PointerWorldPosition = CameraService.Instance.ActiveCamera.ScreenToWorldPoint(Input.mousePosition);
            if (WorldUIEventSystem.Instance != null)
                Cache.PointerInWorld = WorldUIEventSystem.Instance.MouseInWorld;
            Cache.PointedTile = Helpers.GetTile(Cache.PointerWorldPosition);

            Cache.PointedTileActors.Clear();
            Entity tileEntity = Cache.TileWorld.IsCreated ? Cache.TileWorld.GetEntity(Cache.PointedTile) : Entity.Null;
            if (tileEntity != Entity.Null)
            {
                foreach (var tileActor in Cache.SimWorld.GetBufferReadOnly<TileActorReference>(tileEntity))
                {
                    Cache.PointedTileActors.Add(tileActor);
                }
            }
        }
    }
}
