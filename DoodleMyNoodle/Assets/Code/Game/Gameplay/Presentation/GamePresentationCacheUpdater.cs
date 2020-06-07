using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GamePresentationCache
{
    public readonly static GamePresentationCache Instance = new GamePresentationCache();

    public bool Ready = false;

    public Entity LocalPawn = Entity.Null;
    public fix3 LocalPawnPosition = fix3.zero;
    public Vector3 LocalPawnPositionFloat = Vector3.zero;
    public Entity LocalController = Entity.Null;
    public ExternalSimWorldAccessor SimWorld = null;
}

// should we change this to a component system ?
[AlwaysUpdateSystem]
public class GamePresentationCacheUpdater : ViewComponentSystem
{
    GamePresentationCache Cache => GamePresentationCache.Instance;

    protected override void OnCreate()
    {
        base.OnCreate();

        Cache.Ready = true;
        Cache.SimWorld = SimWorldAccessor;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Cache.Ready = false;
        Cache.SimWorld = null;
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
