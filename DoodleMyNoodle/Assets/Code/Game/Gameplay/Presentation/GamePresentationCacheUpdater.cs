using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GamePresentationCache
{
    public readonly static GamePresentationCache Instance = new GamePresentationCache();

    public Entity LocalPawn = Entity.Null;
    public Entity LocalController = Entity.Null;
    public ExternalSimWorldAccessor SimWorld = null;
}

public class GamePresentationCacheUpdater : GameSystem
{
    private Coroutine _fetchSimWorldCoroutine;

    public override bool SystemReady => Cache.SimWorld != null;

    GamePresentationCache Cache => GamePresentationCache.Instance;

    public override void OnGameAwake()
    {
        base.OnGameAwake();

        _fetchSimWorldCoroutine = StartCoroutine(FetchSimWorldReference());
    }

    IEnumerator FetchSimWorldReference()
    {
        while (Cache.SimWorld == null)
        {
            Cache.SimWorld = GameMonoBehaviourHelpers.GetSimulationWorld();
            yield return null;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_fetchSimWorldCoroutine != null)
            StopCoroutine(_fetchSimWorldCoroutine);
    }


    public override void OnGameUpdate()
    {
        UpdateCurrentPlayerPawn();
    }

    private void UpdateCurrentPlayerPawn()
    {
        Cache.LocalPawn = PlayerHelpers.GetLocalSimPawnEntity(Cache.SimWorld);
        Cache.LocalController = CommonReads.GetPawnController(Cache.SimWorld, Cache.LocalPawn);
    }
}
