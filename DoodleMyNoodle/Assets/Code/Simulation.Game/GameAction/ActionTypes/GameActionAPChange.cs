using static fixMath;
using Unity.Entities;
using System;
using System.Collections.Generic;

public class GameActionAPChange : GameAction<GameActionAPChange.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix ToGive;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                ToGive = ToGive,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix ToGive;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        foreach (Entity entity in input.Context.Targets)
        {
            CommonWrites.ModifyStatFix<ActionPoints>(input.Accessor, entity, settings.ToGive);
        }

        return true;
    }
}