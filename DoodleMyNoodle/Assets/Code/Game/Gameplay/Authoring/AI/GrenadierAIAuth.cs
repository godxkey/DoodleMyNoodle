using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class GrenadierAIAuth : AIAuth, IConvertGameObjectToEntity
{
    public FuzzyValue ThrowAngleFuzzy;
    public FuzzyValue ThrowSpeedFuzzy;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.Convert(entity, dstManager, conversionSystem);

        dstManager.AddComponentData(entity, new GrenadierAIData() { });


        AIFuzzyThrowSettings throwSettings = new AIFuzzyThrowSettings()
        {
            ThrowAngle = (FixFuzzyValue)ThrowAngleFuzzy,
            ThrowSpeed = (FixFuzzyValue)ThrowSpeedFuzzy
        };

        throwSettings.ThrowAngle.BaseValue = fixMath.radians(throwSettings.ThrowAngle.BaseValue);
        throwSettings.ThrowAngle.Variation = fixMath.radians(throwSettings.ThrowAngle.Variation);

        dstManager.AddComponentData(entity, throwSettings);
    }
}