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
            {
                var target = input.Context.Targets[i];
                var maxAP = input.Accessor.GetComponent<ActionPointsMax>(target).Value;
                var ap = input.Accessor.GetComponent<ActionPoints>(target).Value;
                var newAP = fixMath.clamp(ap + settings.ToGive, 0, maxAP);
                input.Accessor.SetComponent<ActionPoints>(target, newAP);
            }
        }

        return true;
    }
}