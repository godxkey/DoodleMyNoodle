using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;
using System.Collections.Generic;
using UnityEngineX;

public struct AddGameEffectRequest : ISingletonBufferElementData
{
    public Entity GameEffectPrefab;
    public Entity Target;
    public InstigatorSet Instigator;
}

[AlwaysUpdateSystem]
[UpdateBefore(typeof(ExecuteGameActionSystem))]
public class GameEffectSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        UpdateDurations();

        var addGameEffectRequests = GetSingletonBuffer<AddGameEffectRequest>();
        NativeArray<AddGameEffectRequest> effectRequests = addGameEffectRequests.ToNativeArray(Allocator.Temp);
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

        foreach (var effect in endingEffects)
        {
            if (EntityManager.TryGetComponent(effect, out GameEffectOnEndGameAction gameEffectOnEndGameAction))
            {
                CommonWrites.RequestExecuteGameAction(Accessor, effect, gameEffectOnEndGameAction.Action, GetComponent<GameEffectInfo>(effect).Owner);
            }

            CommonWrites.DestroyEndOfTick(Accessor, effect);
        }
    }

    private void ProcessAddRequests(NativeArray<AddGameEffectRequest> addRequests)
    {
        foreach (AddGameEffectRequest addRequest in addRequests)
        {
            // check that target has a buffer for it. This will also check if the target has been destroyed
            if (!EntityManager.HasComponent<GameEffectBufferElement>(addRequest.Target))
                continue;

            // _________________________________________ Create Effect _________________________________________ //
            Entity newEffect = EntityManager.Instantiate(addRequest.GameEffectPrefab);

            EntityManager.AddComponentData(newEffect, new GameEffectInfo()
            {
                Instigator = addRequest.Instigator,
                Owner = addRequest.Target,
            });

            // FRED: Pas sur que ça devrait être la target. Me semble que l'instigateur devrait être la personne qui a mis l'effect. E.g. si j'met qq1 en feu et que 
            // ça fait du dots, ça devrait checker mon 'bonus-fire-damage' ?
            EntityManager.AddComponentData(newEffect, new FirstInstigator() { Value = addRequest.Target });


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

internal static partial class CommonWrites
{
    static public void AddGameEffect(ISimGameWorldReadWriteAccessor accessor, AddGameEffectRequest request)
    {
        accessor.GetSingletonBuffer<AddGameEffectRequest>().Add(request);
    }
}