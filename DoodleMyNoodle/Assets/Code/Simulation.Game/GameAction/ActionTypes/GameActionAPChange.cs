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
        for (int i = 0; i < input.Context.Targets.Length; i++)
        {
            if (input.Accessor.HasComponent<ActionPoints>(input.Context.Targets[i]))
                CommonWrites.ModifyStatFix<ActionPoints>(input.Accessor, input.Context.Targets[i], settings.ToGive);
        }

        return true;
    }
}