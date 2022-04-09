using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;
using System.Collections.Generic;

public struct AddGameEffectRequest : ISingletonBufferElementData
{
    public Entity GameEffect;
    public Entity Target;
    public Entity Instigator;
}

[AlwaysUpdateSystem]
[UpdateBefore(typeof(ExecuteGameActionSystem))]
public class GameEffectSystem : SimGameSystemBase
{
    List<Entity> EntitiesGameEffectToUpdate = new List<Entity>();

    protected override void OnUpdate()
    {
        UpdateDurations();

        var addGameEffectRequests = GetSingletonBuffer<AddGameEffectRequest>();
        NativeArray<AddGameEffectRequest> effectRequests = addGameEffectRequests.ToNativeArray(Allocator.Temp);
        addGameEffectRequests.Clear();

        ProcessAddRequests(effectRequests);
        effectRequests.Dispose();
    }

    private void UpdateDurations()
    {
        fix deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity entity, ref GameEffect effect) =>
        {
            effect.RemainingTime -= deltaTime;

            if (effect.RemainingTime < 0)
            {
                EntitiesGameEffectToUpdate.Remove(entity);

                if (EntityManager.TryGetComponent(entity, out GameEffectOnEndGameAction gameEffectOnEndGameAction))
                {
                    CommonWrites.RequestExecuteGameAction(Accessor, entity, gameEffectOnEndGameAction.Action);
                }

                CommonWrites.DestroyEndOfTick(Accessor, entity);
            }
        }).WithoutBurst().Run();
    }

    private void ProcessAddRequests(NativeArray<AddGameEffectRequest> addGameEffectRequests)
    {
        // Spawn new game effect entity
        List<Entity> newEntities = new List<Entity>();

        foreach (AddGameEffectRequest addRequest in addGameEffectRequests)
        {
            EntitiesGameEffectToUpdate.Add(addRequest.Target);

            Entity newGameEffectEntity = EntityManager.Instantiate(addRequest.GameEffect);

            EntityManager.AddComponentData(newGameEffectEntity, new GameEffectInfo()
            {
                Instigator = addRequest.Instigator,
                Owner = addRequest.Target
            });

            if (EntityManager.TryGetComponent(newGameEffectEntity, out GameEffectOnBeginGameAction gameEffectOnBeginGameAction))
            {
                CommonWrites.RequestExecuteGameAction(Accessor, newGameEffectEntity, gameEffectOnBeginGameAction.Action);
            }

            newEntities.Add(newGameEffectEntity);
        }

        // setup game effect entity reference
        foreach (Entity entity in newEntities)
        {
            if (EntityManager.TryGetComponent(entity, out GameEffectInfo gameEffectInfo))
            {
                if (EntityManager.TryGetBuffer(gameEffectInfo.Owner, out DynamicBuffer<GameEffectBufferElement> effects))
                {
                    effects.Add(new GameEffectBufferElement()
                    {
                        EffectEntity = entity
                    });
                }
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