using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GamePresentationCache
{
    public readonly static GamePresentationCache Instance = new GamePresentationCache();

    public bool Ready;

    public Entity LocalPawn;
    public fix3 LocalPawnPosition;
    public Vector3 LocalPawnPositionFloat;
    public Entity LocalController;
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

        ResetCache();
        Cache.Ready = true;
        Cache.SimWorld = SimWorldAccessor;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        ResetCache();
        Cache.Ready = false;
        Cache.SimWorld = null;
    }

    private void ResetCache()
    {
        Cache.LocalController = Entity.Null;
        Cache.LocalPawn = Entity.Null;
    }

    protected override void OnUpdate()
    {
        UpdateCurrentPlayerPawn();
    }

    private void UpdateCurrentPlayerPawn()
    {
        Cache.LocalPawn = PlayerHelpers.GetLocalSimPawnEntity(Cache.SimWorld);
        Cache.LocalController = CommonReads.GetPawnController(Cache.SimWorld, Cache.LocalPawn);
        if(Cache.LocalPawn != Entity.Null)
        {
            Cache.LocalPawnPosition = Cache.SimWorld.GetComponentData<FixTranslation>(Cache.LocalPawn).Value;
            Cache.LocalPawnPositionFloat = Cache.LocalPawnPosition.ToUnityVec();
        }
    }
}
