using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class InstantiateStartingStatusEffectSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<StatusEffect>()
            .ForEach((Entity entity, DynamicBuffer<StatusEffect> statusEffectBuffer, DynamicBuffer <StartingStatusEffect> startingStatusEffectBuffer) =>
            {
                NativeArray<StartingStatusEffect> startingStatusEffect = startingStatusEffectBuffer.ToNativeArray(Allocator.Temp);

                for (int i = 0; i < startingStatusEffect.Length; i++)
                {
                    var statusEffect = startingStatusEffect[i];
                    CommonWrites.AddStatusEffect(Accessor, new AddStatusEffectRequest() { Target = entity, Instigator = entity, Type = statusEffect.Type, StackAmount = statusEffect.StackAmount });
                }

                // Remove starting inventory buffer
                EntityManager.RemoveComponent<StartingStatusEffect>(entity);
            })
            .WithoutBurst()
            .WithStructuralChanges()
            .Run();
    }
}
