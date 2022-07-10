using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class MessageAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public string Value;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Message()
        {
            Value = new FixedString64Bytes(this.Value)
        });
    }
}
