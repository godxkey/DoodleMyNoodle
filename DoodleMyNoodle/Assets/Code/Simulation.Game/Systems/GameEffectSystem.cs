using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;
using System.Collections.Generic;
using UnityEngineX;

public struct SystemRequestAddGameEffect : ISingletonBufferElementData
{
    public Entity GameEffectPrefab;
    public Entity Target;
    public Entity Instigator;
}

[AlwaysUpdateSystem]
[UpdateBefore(typeof(ExecuteGameActionSystem))]
public partial class GameEffectSystem : SimGameSystemBase
{
    private RemoveFinishedGameEffectsSystem _removeFinishedGameEffectsSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _removeFinishedGameEffectsSystem = World.GetOrCreateSystem<RemoveFinishedGameEffectsSystem>();
    }

    protected override void OnUpdate()
    {
        UpdateDurations();

        var addGameEffectRequests = GetSingletonBuffer<SystemRequestAddGameEffect>();

        // Default Start Buffer element
        Entities.ForEach((Entity entity, DynamicBuffer<GameEffectStartBufferElement> gameEffectStartBufferElements) =>
        {
            foreach (var gameEffectStartBufferElement in gameEffectStartBufferElements)
            {
                addGameEffectRequests.Add(new SystemRequestAddGameEffect()
                {
                    GameEffectPrefab = gameEffectStartBufferElement.EffectEntity,
                    Target = entity,
                    Instigator = entity,
                });
            }

            gameEffectStartBufferElements.Clear();

        }).Run();

        NativeArray<SystemRequestAddGameEffect> effectRequests = addGameEffectRequests.ToNativeArray(Allocator.Temp);
        addGameEffectRequests.Clear();

        ProcessAddRequests(effectRequests);
    }

    private void UpdateDurations()
    {
        fix deltaTime = Time.DeltaTime;

        NativeList<Entity> endingEffects = new NativeList<Entity>(Allocator.Temp);

        Entities.ForEach((Entity effect, ref GameEffectRemainingDuration remainingDuration) =>
        {
            remainingDuration.Value -= deltaTime;

            if (remainingDuration.Value < 0)
            {
                endingEffects.Add(effect);
            }

        }).Run();

        for (int i = 0; i < endingEffects.Length; i++)
        {
            var effect = endingEffects[i];
            var effectOwner = GetComponent<Owner>(effect);

            if (EntityManager.TryGetComponent(effect, out GameEffectOnEndGameAction gameEffectOnEndGameAction))
            {
                CommonWrites.RequestExecuteGameAction(Accessor, effect, gameEffectOnEndGameAction.Action, effectOwner);
            }

            _removeFinishedGameEffectsSystem.ToRemove.Add((effectOwner, effect));

            CommonWrites.DisableAndScheduleForDestruction(Accessor, effect);
        }
    }

    private void ProcessAddRequests(NativeArray<SystemRequestAddGameEffect> addRequests)
    {
        foreach (SystemRequestAddGameEffect addRequest in addRequests)
        {
            // check that target has a buffer for it. This will also check if the target has been destroyed
            if (!EntityManager.HasComponent<GameEffectBufferElement>(addRequest.Target))
                continue;

            if (addRequest.GameEffectPrefab == Entity.Null)
            {
                Log.Warning("Trying to add a GameEffect with a null prefab. Request skipped.");
                continue;
            }

            // _________________________________________ Create Effect _________________________________________ //
            Entity newEffect = EntityManager.Instantiate(addRequest.GameEffectPrefab);

            EntityManager.AddComponentData<FirstInstigator>(newEffect, CommonReads.GetFirstInstigator(Accessor, addRequest.Instigator));

            // FRED: Pas sur que ?a devrait ?tre la target. Me semble que l'instigateur devrait ?tre la personne qui a mis l'effect. E.g. si j'met qq1 en feu et que 
            // ?a fait du dots, ?a devrait checker mon 'bonus-fire-damage' ?
            EntityManager.AddComponentData(newEffect, new Owner() { Value = addRequest.Target });

            // _________________________________________ Add to owner _________________________________________ //
            DynamicBuffer<GameEffectBufferElement> effects = EntityManager.GetBuffer<GameEffectBufferElement>(addRequest.Target);

            effects.Add(new GameEffectBufferElement()
            {
                EffectEntity = newEffect
            });

            // _________________________________________ Execute GameAction _________________________________________ //
            if (EntityManager.TryGetComponent(newEffect, out GameEffectOnBeginGameAction beginAction))
            {
                CommonWrites.RequestExecuteGameAction(Accessor, newEffect, beginAction.Action, addRequest.Target);
            }
        }
    }
}


[AlwaysUpdateSystem]
[UpdateAfter(typeof(ExecuteGameActionSystem))]
public partial class RemoveFinishedGameEffectsSystem : SimGameSystemBase
{
    public List<(Entity owner, Entity effect)> ToRemove = new List<(Entity owner, Entity effect)>();

    protected override void OnUpdate()
    {
        foreach ((Entity owner, Entity effect) in ToRemove)
        {
            if (EntityManager.TryGetBuffer<GameEffectBufferElement>(owner, out var buffer))
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i].EffectEntity == effect)
                    {
                        buffer.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        ToRemove.Clear();
    }
}

internal static partial class CommonWrites
{
    static public void AddGameEffect(ISimGameWorldReadWriteAccessor accessor, SystemRequestAddGameEffect request)
    {
        accessor.GetSingletonBuffer<SystemRequestAddGameEffect>().Add(request);
    }
}