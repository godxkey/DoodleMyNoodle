using Unity.Entities;
using System;

public class GameActionDisablePeriodicAction : GameAction<GameActionDisablePeriodicAction.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public bool ShouldDisable = true;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings() { ShouldDisable = ShouldDisable });
        }
    }

    public struct Settings : IComponentData 
    {
        public bool ShouldDisable;
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

            if (input.Accessor.HasComponent<PeriodicActionEnabled>(target))
            {
                input.Accessor.SetComponent<PeriodicActionEnabled>(target, false);
                input.Accessor.SetComponent<RemainingPeriodicActionCount>(target, 0);
            }
        }

        return true;
    }
}