using SimulationControl;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using CCC.Fix2D;
using UnityEngineX;
using Unity.Collections;

public class GamePresentationCache
{
    public readonly static GamePresentationCache Instance = new GamePresentationCache();

    public bool Ready;

    public Vector2 CameraPosition;
    public float CameraSize;
    public bool PointerInWorld;
    public Vector2 PointerWorldPosition;
    public int2 PointedTile;
    public List<Entity> PointedBodies = new List<Entity>();
    public List<BindedSimEntityManaged> PointedViewEntities = new List<BindedSimEntityManaged>();
    public List<Collider2D> PointedColliders = new List<Collider2D>();
    public List<GameObject> PointedGameObjects = new List<GameObject>();

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

    private Collider2D[] _overlapResults = new Collider2D[64];

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
        Cache.PointedBodies.Clear();
        Cache.PointedViewEntities.Clear();
        Cache.PointedColliders.Clear();
        Cache.PointedGameObjects.Clear();
    }

    protected override void OnUpdate()
    {
        ////////////////////////////////////////////////////////////////////////////////////////
        //      Camera
        ////////////////////////////////////////////////////////////////////////////////////////
        if (CameraMovementController.Instance != null)
        {
            Cache.CameraPosition = CameraMovementController.Instance.CamPosition;
            Cache.CameraSize = CameraMovementController.Instance.CamSize;
        }

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


            int hitCount = Physics2D.OverlapPointNonAlloc(Cache.PointerWorldPosition, _overlapResults, layerMask: ~0);

            Cache.PointedViewEntities.Clear();
            Cache.PointedColliders.Clear();
            Cache.PointedGameObjects.Clear();
            for (int i = 0; i < hitCount; i++)
            {
                Cache.PointedColliders.Add(_overlapResults[i]);
                Cache.PointedGameObjects.AddUnique(_overlapResults[i].gameObject);

                if (_overlapResults[i].gameObject.TryGetComponent(out BindedSimEntityManaged bindedSimEntity))
                {
                    Cache.PointedViewEntities.Add(bindedSimEntity);
                }
            }


            Cache.PointedBodies.Clear();
            var physicsWorldSys = Cache.SimWorld.GetExistingSystem<PhysicsWorldSystem>();
            if (physicsWorldSys.PhysicsWorldFullyUpdated)
            {
                NativeList<OverlapPointHit> hits = new NativeList<OverlapPointHit>(Allocator.Temp);
                OverlapPointInput input = OverlapPointInput.Default;
                input.Position = Cache.PointerWorldPosition;
                physicsWorldSys.PhysicsWorld.OverlapPoint(input, ref hits);
                foreach (var hit in hits)
                {
                    Cache.PointedBodies.Add(hit.Entity);
                }
            }
        }
    }
}
