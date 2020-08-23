using Unity.Entities;
using UnityEngine;


[DisallowMultipleComponent]
[RequiresEntityConversion]
public abstract class AIAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<AITag>(entity);
        dstManager.AddComponent<ControlledEntity>(entity);
        dstManager.AddComponentData(entity, new ReadyForNextTurn() { Value = false });
    }
}
