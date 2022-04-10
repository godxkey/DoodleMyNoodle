using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class InstantiateStartingStatModifiersSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<StatModifier>()
            .ForEach((Entity entity, DynamicBuffer<StatModifier> statusEffectBuffer, DynamicBuffer <StartingStatModifier> startingStatusEffectBuffer) =>
            {
                NativeArray<StartingStatModifier> startingStatusEffect = startingStatusEffectBuffer.ToNativeArray(Allocator.Temp);

                for (int i = 0; i < startingStatusEffect.Length; i++)
                {
                    var statusEffect = startingStatusEffect[i];
                    CommonWrites.AddStatusEffect(Accessor, new AddStatModifierRequest() { Target = entity, Instigator = entity, Type = statusEffect.Type, StackAmount = statusEffect.StackAmount });
                }

                // Remove starting inventory buffer
                EntityManager.RemoveComponent<StartingStatModifier>(entity);
            })
            .WithoutBurst()
            .WithStructuralChanges()
            .Run();
    }
}
