using System;
using Unity.Entities;
using UnityEngine;

public enum TypeActorType
{
    Terrain, // Might not need this here in the future !!
    Ladder, // Might not need this here in the future !!

    Dynamic,
    Static
}

[DisallowMultipleComponent]
public class TileActorAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private TypeActorType _type = TypeActorType.Dynamic;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<TileActorTag>(entity);

        switch (_type)
        {
            case TypeActorType.Dynamic:
                //dstManager.AddComponentData(entity, new Velocity { Value = new fix2(0, 0) });
                //dstManager.AddComponentData(entity, new PotentialNewTranslation { Value = new fix2(0, 0) });
                break;

            case TypeActorType.Static:
                //dstManager.AddComponent<StaticTag>(entity);
                break;

            default:
                throw new NotImplementedException();
        }
    }

    public bool ShouldBeConvertedToTile()
    {
        switch (_type)
        {
            case TypeActorType.Terrain:
            case TypeActorType.Ladder:
                return true;

            default:
                return false;
        }
    }

    public TileFlagComponent GetTileFlags()
    {
        switch (_type)
        {
            case TypeActorType.Terrain:
                return new TileFlagComponent() { Value = TileFlags.Terrain };
            
            case TypeActorType.Ladder:
                return new TileFlagComponent() { Value = TileFlags.Ladder };

            default:
                return default;
        }
    }
}
