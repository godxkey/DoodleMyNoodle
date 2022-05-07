using static fixMath;
using Unity.Entities;
using System;
using CCC.Fix2D;
using CCC.InspectorDisplay;
using UnityEngine;
using System.Collections.Generic;

public class GameActionRadialImpulse : GameAction<GameActionRadialImpulse.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public float Radius = 1;
        public float MinStrenght = 1;
        public float MaxStrenght = 1;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Radius = (fix)Radius,
                MinStrenght = (fix)MinStrenght,
                MaxStrenght = (fix)MaxStrenght
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Radius;
        public fix MinStrenght;
        public fix MaxStrenght;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings) => null;

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        if (input.Accessor.TryGetComponent(input.Context.LastPhysicalInstigator, out FixTranslation fixTranslation))
        {
            fix2 radialImpulsePosition = fixTranslation.Value;
            for (int i = 0; i < input.Context.Targets.Length; i++)
            {
                var target = input.Context.Targets[i];

                CommonWrites.RequestRadialImpulse(input.Accessor, target, settings.MinStrenght, settings.MaxStrenght, settings.Radius, radialImpulsePosition, true);
            }
        }

        return true;
    }
}