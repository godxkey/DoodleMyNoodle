using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class NameAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public string Value;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Name()
        {
            Value = new FixedString64(this.Value)
        });
    }
}
