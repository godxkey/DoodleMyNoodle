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
    public fix2 LocalPawnPosition;
    public Vector2 LocalPawnPositionFloat;
    public Entity LocalController;
    public fix GroupHealth;
    public fix GroupShield;
    public ExternalSimGameWorldAccessor SimWorld;

    public bool LocalPawnExists => LocalPawn != Entity.Null;
    public bool LocalControllerExists => LocalController != Entity.Null;
    public bool LocalPawnAlive => LocalPawnExists && GroupHealth > 0;

    public int2 DEPRECATED_LocalPawnTile;
    public Entity DEPRECATED_LocalPawnTileEntity;
    public Team DEPRECATED_LocalControllerTeam = 1;
    public bool DEPRECATED_CanLocalPlayerPlay = true;
    public bool DEPRECATED_IsNewTurn = false;
    public bool DEPRECATED_IsNewRound = false;
    public List<Entity> DEPRECATED_CurrentlyPlayingControllers = new List<Entity>();
    public TileWorld DEPRECATED_TileWorld;
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
        Cache.SimWorld = (ExternalSimGameWorldAccessor)SimWorldAccessor;
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
        //      Player & Pawn
        ////////////////////////////////////////////////////////////////////////////////////////
        Cache.LocalPawn = PlayerHelpers.GetLocalSimPawnEntity(Cache.SimWorld);

        if (Cache.LocalPawn != Entity.Null)
        {
            Cache.LocalController = CommonReads.TryGetPawnController(Cache.SimWorld, Cache.LocalPawn);
            Cache.LocalPawnPosition = Cache.SimWorld.GetComponent<FixTranslation>(Cache.LocalPawn).Value;
            Cache.LocalPawnPositionFloat = Cache.LocalPawnPosition.ToUnityVec();
        }
        else
        {
            Cache.LocalController = Entity.Null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Player Group
        ////////////////////////////////////////////////////////////////////////////////////////
        if (Cache.SimWorld.HasSingleton<PlayerGroupDataTag>())
        {
            var playerGroupEntity = Cache.SimWorld.GetSingletonEntity<PlayerGroupDataTag>();
            Cache.GroupHealth = Cache.SimWorld.GetComponent<Health>(playerGroupEntity);
            Cache.GroupShield = Cache.SimWorld.GetComponent<Shield>(playerGroupEntity);
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
