using Unity.Entities;
using UnityEngine;


[DisallowMultipleComponent]
[RequiresEntityConversion]
public abstract class AIAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public DesignerFriendlyTeam StartingTeam = DesignerFriendlyTeam.Baddies;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Team() { Value = (int)StartingTeam });
        dstManager.AddComponent<Team>(entity);
        dstManager.AddComponent<AITag>(entity);
        dstManager.AddComponent<ControlledEntity>(entity);
        dstManager.AddComponentData<Active>(entity, true);
        dstManager.AddComponentData(entity, new ReadyForNextTurn() { Value = false });
    }
}
