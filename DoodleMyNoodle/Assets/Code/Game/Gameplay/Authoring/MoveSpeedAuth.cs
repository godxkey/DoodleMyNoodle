using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class MoveSpeedAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public fix Value;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveSpeed { Value = Value });
    }
}
