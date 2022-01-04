using CCC.InspectorDisplay;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class DelayedExplosionShieldAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool UseTime = true;
    [ShowIf("UseTime")]
    public int TimeDelay = 1;
    [HideIf("UseTime")]
    public int TurnDelay = 1;

    public fix Radius = 1;
    public int RoundDuration = 1;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DelayedExplosionShield()
        {
            UseTime = UseTime,
            TimeDuration = TimeDelay,
            TurnDuration = TurnDelay,
            Radius = Radius,
            RoundDuration = RoundDuration
        });
    }
}