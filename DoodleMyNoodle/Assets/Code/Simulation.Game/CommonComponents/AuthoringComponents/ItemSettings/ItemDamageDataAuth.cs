using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemDamageDataAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Damage;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemDamageData() { Value = Damage });
    }
}