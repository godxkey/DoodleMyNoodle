using Unity.Entities;
using UnityEngine;


[DisallowMultipleComponent]
public abstract class AIAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Team>(entity);
        dstManager.AddComponent<AITag>(entity);
        dstManager.AddComponent<ControlledEntity>(entity);
        dstManager.AddComponentData<Active>(entity, true);
        dstManager.AddComponentData(entity, new ReadyForNextTurn() { Value = false });
        dstManager.AddComponent<AIDestination>(entity);
        dstManager.AddComponent<AIDestinationRepathData>(entity);
        dstManager.AddBuffer<AIPathSegment>(entity);
        dstManager.AddComponent<AIMoveInputLastFrame>(entity);
        dstManager.AddComponent<AIActionCooldown>(entity);
        dstManager.AddComponent<AIThinksThisFrameToken>(entity);
        dstManager.AddComponent<AIPatrolData>(entity);
        dstManager.AddComponent<AIState>(entity);
    }
}
