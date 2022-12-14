using System.Collections.Generic;
using System;
using Unity.Entities;

public class GameActionDestroySelf : GameAction<GameActionDestroySelf.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
            });
        }
    }

    public struct Settings : IComponentData
    {
        public bool DeleteMeWhenYouAddOtherStuff; // this is needed to avoid 'Zero sized component' exception
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        CommonWrites.DisableAndScheduleForDestruction(input.Accessor, input.ActionInstigatorActor);

        return true;
    }
}