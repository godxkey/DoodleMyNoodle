using static fixMath;
using Unity.Entities;
using System;
using System.Collections.Generic;
using UnityEngineX;
using Unity.Collections;

public class GameActionRefillSpellCharge : GameAction<GameActionRefillSpellCharge.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public int NumberOfCharges = 1;
        public int SpecificIndex = -1;
        public bool Self = false;
        public bool AdjacentLeft = false;
        public bool AdjacentRight = false;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                NumberOfCharges = NumberOfCharges,
                SpecificIndex = SpecificIndex,
                Self = Self,
                AdjacentLeft = AdjacentLeft,
                AdjacentRight = AdjacentRight
            });
        }
    }

    public struct Settings : IComponentData
    {
        public int NumberOfCharges;
        public int SpecificIndex;
        public bool Self;
        public bool AdjacentLeft;
        public bool AdjacentRight;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings) => null;

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        Entity FirstInstigator = input.Context.InstigatorSet.FirstPhysicalInstigator;
        if (input.Accessor.TryGetComponent(input.Context.ActionInstigator, out GameEffectInfo gameEffectInfo))
        {
            FirstInstigator = gameEffectInfo.Instigator.FirstPhysicalInstigator;
        }

        int spellIndex = CommonReads.FindItemIndex(input.Accessor, FirstInstigator, input.Context.InstigatorSet.LastSpellInstigator);

        if (settings.Self)
        {
            Entity LastSpellEntity = input.Context.InstigatorSet.LastSpellInstigator;
            if (input.Accessor.TryGetComponent(LastSpellEntity, out ItemCharges itemCharges))
            {
                itemCharges.Value += settings.NumberOfCharges;
                input.Accessor.SetComponent(LastSpellEntity, itemCharges);
            }
        }

        if (settings.AdjacentLeft)
        {
            Entity leftItemEntity = CommonReads.FindItemByIndex(input.Accessor, FirstInstigator, spellIndex - 1);
            if (input.Accessor.TryGetComponent(leftItemEntity, out ItemCharges itemCharges))
            {
                itemCharges.Value += settings.NumberOfCharges;
                input.Accessor.SetComponent(leftItemEntity, itemCharges);
            }
        }

        if (settings.AdjacentRight)
        {
            Entity rightItemEntity = CommonReads.FindItemByIndex(input.Accessor, FirstInstigator, spellIndex + 1);
            if (input.Accessor.TryGetComponent(rightItemEntity, out ItemCharges itemCharges))
            {
                itemCharges.Value += settings.NumberOfCharges;
                input.Accessor.SetComponent(rightItemEntity, itemCharges);
            }
        }

        if (settings.SpecificIndex >= 0)
        {
            Entity specificIndexItemEntity = CommonReads.FindItemByIndex(input.Accessor, FirstInstigator, settings.SpecificIndex);
            if (input.Accessor.TryGetComponent(specificIndexItemEntity, out ItemCharges itemCharges))
            {
                itemCharges.Value += settings.NumberOfCharges;
                input.Accessor.SetComponent(specificIndexItemEntity, itemCharges);
            }
        }

        return true;
    }
}