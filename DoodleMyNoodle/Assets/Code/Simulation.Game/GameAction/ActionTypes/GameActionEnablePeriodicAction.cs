using static fixMath;
using Unity.Entities;
using System;
using System.Collections.Generic;

public class GameActionEnablePeriodicAction : GameAction<GameActionEnablePeriodicAction.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public int UsageCount;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                UsageCount = UsageCount,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public int UsageCount;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        for(int i = 0; i < input.Context.Targets.Length; i++)
        {
            var target = input.Context.Targets[i];

            if (input.Accessor.HasComponent<PeriodicActionEnabled>(target))
            {
                input.Accessor.SetComponent<PeriodicActionEnabled>(target, true);
                input.Accessor.SetComponent<RemainingPeriodicActionCount>(target, settings.UsageCount);
            }
        }

        return true;
    }
}