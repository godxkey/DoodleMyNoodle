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
        public float Duration = -1;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                UsageCount = UsageCount,
                Duration = (fix)Duration,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public int UsageCount;
        public fix Duration;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        if (input.Accessor.HasComponent<PeriodicActionEnabled>(input.Context.ActionInstigator))
        {
            input.Accessor.SetComponent<PeriodicActionEnabled>(input.Context.ActionInstigator, true);
            input.Accessor.SetComponent<PeriodicActionTimer>(input.Context.ActionInstigator, settings.Duration);
            input.Accessor.SetComponent<RemainingPeriodicActionCount>(input.Context.ActionInstigator, settings.UsageCount);
        }

        return true;
    }
}