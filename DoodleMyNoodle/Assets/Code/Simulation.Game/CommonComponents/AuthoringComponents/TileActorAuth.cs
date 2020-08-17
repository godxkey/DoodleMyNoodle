using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct Velocity : IComponentData
{
    public fix3 Value;

    public static implicit operator fix3(Velocity velocity) => velocity.Value;
    public static implicit operator Velocity(fix3 velocity) => new Velocity() { Value = velocity };
}

public struct TileActorTag : IComponentData { }
public struct StaticTag : IComponentData { }

// TODO: This should not be needed!
public struct TerrainTag : IComponentData { }
public struct LadderTag : IComponentData { }

public enum TypeActorType
{
    Terrain, // Might not need this here in the future !!
    Ladder, // Might not need this here in the future !!

    Dynamic,
    Static
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class TileActorAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private TypeActorType _type = TypeActorType.Dynamic;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<TileActorTag>(entity);

        switch (_type)
        {
            case TypeActorType.Dynamic:
                dstManager.AddComponentData(entity, new Velocity { Value = new fix3(0, 0, 0) });
                dstManager.AddComponentData(entity, new PotentialNewTranslation { Value = new fix3(0, 0, 0) });
                break;

            case TypeActorType.Static:
                dstManager.AddComponent<StaticTag>(entity);
                break;

            case TypeActorType.Terrain:
                dstManager.AddComponent<StaticTag>(entity);
                dstManager.AddComponent<TerrainTag>(entity);
                break;

            case TypeActorType.Ladder:
                dstManager.AddComponent<StaticTag>(entity);
                dstManager.AddComponent<LadderTag>(entity);
                break;
        }
    }
}
