using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class GameOverDestinationToReachSingletonAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameOverDestinationToReachSingleton { XPosition = (fix)transform.position.x });
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(0,-50,0), transform.position + new Vector3(0, 50, 0));
    }
}

