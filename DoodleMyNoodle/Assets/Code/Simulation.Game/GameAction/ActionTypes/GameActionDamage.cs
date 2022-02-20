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

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
        // n.b. maybe entity or position in the future ?
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        foreach (Entity target in input.Context.Targets)
        {
            CommonWrites.RequestDamage(input.Accessor, target, settings.Damage);

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