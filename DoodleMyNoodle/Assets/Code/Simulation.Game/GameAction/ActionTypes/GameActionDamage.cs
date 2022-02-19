using static fixMath;
using Unity.Entities;
using System;
using System.Collections.Generic;
using CCC.Fix2D;

public class GameActionDamage : GameAction<GameActionDamage.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Damage;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Damage = Damage
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Damage;
    }

    public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Settings settings)
    {
        return new ExecutionContract();
        // n.b. maybe entity or position in the future ?
    }

    public override bool Use(ISimGameWorldReadWriteAccessor accessor, in ExecutionContext context, UseParameters useData, List<ResultDataElement> resultData, Settings settings)
    {
        foreach (Entity target in context.Targets)
        {
            CommonWrites.RequestDamage(accessor, target, settings.Damage);

            if (accessor.TryGetComponent(target, out FixTranslation targetTranslation))
            {
                resultData.Add(new ResultDataElement()
                {
                    Position = targetTranslation.Value,
                    Entity = target
                });
            }
        }

        return true;
    }
}