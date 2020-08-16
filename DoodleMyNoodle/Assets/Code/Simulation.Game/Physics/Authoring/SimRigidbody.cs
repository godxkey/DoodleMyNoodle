using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class SimRigidbody : MonoBehaviour, IConvertGameObjectToEntity
{
    public enum Type
    {
        Terrain = 0,
        Others = 1
    }

    [SerializeField] private Vector2 _colliderSize = new Vector2(0.5f, 1f);
    [SerializeField] private Type _type;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        switch (_type)
        {
            case Type.Terrain:
                dstManager.AddComponentData(entity, new PhysicsCollider()
                {
                    Size = (fix2)_colliderSize,
                    BelongsTo = PhysicsLayer.Terrain,
                    CollidesWith = PhysicsLayer.Unit | PhysicsLayer.Terrain,
                    TriggersWith = PhysicsLayer.None
                });
                break;

            case Type.Others:
                dstManager.AddComponentData(entity, new PhysicsCollider()
                {
                    Size = (fix2)_colliderSize,
                    BelongsTo = PhysicsLayer.Unit,
                    CollidesWith = PhysicsLayer.Terrain,
                    TriggersWith = PhysicsLayer.Unit
                });
                dstManager.AddComponent<Velocity>(entity);
                dstManager.AddComponentData(entity, new PhysicsGravity() { Scale = 1 });
                break;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawWireCube(transform.position, _colliderSize);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, _colliderSize);
    }
#endif
}