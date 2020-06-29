using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemCooldownDataAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Cooldown;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemCooldownData() { Value = Cooldown });
    }
}