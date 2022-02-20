using Unity.Entities;
using System;
using CCC.Fix2D;

public class GameActionFreeze : GameAction<GameActionFreeze.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Duration;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Duration = Duration
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Duration;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
        // n.b. maybe entity or position in the future ?
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        TimeValue elapsedTimeSeconds = TimeValue.Seconds(input.Accessor.Time.ElapsedTime);

        foreach (Entity target in input.Context.Targets)
        {
            input.Accessor.SetOrAddComponent(target, new Frozen()
            {
                AppliedTime = elapsedTimeSeconds,
                Duration = TimeValue.Seconds(settings.Duration)
            });

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