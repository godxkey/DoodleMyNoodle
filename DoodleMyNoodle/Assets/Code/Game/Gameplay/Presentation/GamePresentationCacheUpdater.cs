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
    }

    protected override void OnUpdate()
    {
        UpdateCurrentPlayerPawn();

        Cache.CurrentTeam = new Team() { Value = Cache.SimWorld.GetSingleton<TurnCurrentTeam>().Value };
    }

    private void UpdateCurrentPlayerPawn()
    {
        Cache.LocalPawn = PlayerHelpers.GetLocalSimPawnEntity(Cache.SimWorld);
        Cache.LocalController = CommonReads.GetPawnController(Cache.SimWorld, Cache.LocalPawn);

        if (Cache.SimWorld.TryGetComponentData(Cache.LocalController, out Team pawnTeam))
        {
            Cache.LocalPawnTeam = pawnTeam;
        }

        if (Cache.LocalPawn != Entity.Null)
        {
            Cache.LocalPawnPosition = Cache.SimWorld.GetComponentData<FixTranslation>(Cache.LocalPawn).Value;
            Cache.LocalPawnPositionFloat = Cache.LocalPawnPosition.ToUnityVec();
            Cache.LocalPawnTile = Helpers.GetTile(Cache.LocalPawnPosition);
            Cache.LocalPawnTileEntity = CommonReads.GetTileEntity(Cache.SimWorld, Cache.LocalPawnTile);
        }
        else
        {
            Cache.LocalPawnTileEntity = Entity.Null;
        }
    }
}
