using static fixMath;
using Unity.Entities;
using System;

public class GameActionKnockback : GameAction<GameActionKnockback.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public float Strength;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Strength = (fix)Strength,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Strength;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return null;
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        var impulseRequests = input.Accessor.GetSingletonBuffer<SystemRequestImpulseDirect>();
        var targets = input.Context.Targets;
        for (int i = 0; i < targets.Length; i++)
        {
            impulseRequests.Add(new SystemRequestImpulseDirect()
            {
                IgnoreMass = true,
                Strength = fix2(settings.Strength, 0),
                Target = targets[i],
            });
        }
        return true;
    }
}
