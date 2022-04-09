using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class GameEffectContainerAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<GameEffectBufferElement>(entity);
    }
}