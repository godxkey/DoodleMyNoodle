using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class LassoAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public fix PullSpeed;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new LassoData()
        {
            PullSpeed = PullSpeed
        }); ;
    }
}