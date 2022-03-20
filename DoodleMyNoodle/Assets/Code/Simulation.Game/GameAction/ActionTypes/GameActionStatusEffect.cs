﻿using Unity.Entities;
using System;
using CCC.Fix2D;

public class GameActionStatusEffect : GameAction<GameActionStatusEffect.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public StatusEffectType Type;
        public int StackAmount;
        public bool Remove;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Type = Type,
                StackAmount = StackAmount,
                Remove = Remove
            });
        }
    }

    public struct Settings : IComponentData
    {
        public StatusEffectType Type;
        public int StackAmount;
        public bool Remove;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        for (int i = 0; i < input.Context.Targets.Length; i++)
        {
            var target = input.Context.Targets[i];

            if (settings.Remove)
            {
                CommonWrites.RemoveStatusEffect(input.Accessor, new RemoveStatusEffectRequest() { Target = target, Type = settings.Type, StackAmount = settings.StackAmount, Instigator = input.Context.LastPhysicalInstigator });
            }
            else
            {
                CommonWrites.AddStatusEffect(input.Accessor, new AddStatusEffectRequest() { Target = target, Type = settings.Type, StackAmount = settings.StackAmount, Instigator = input.Context.LastPhysicalInstigator });
            }

            if (input.Accessor.TryGetComponent(target, out FixTranslation targetTranslation))
            {
                output.ResultData.Add(new ResultDataElement()
                {
                    Position = targetTranslation.Value,
                    Entity = target
                });
            }
        }

        return true;
    }
}