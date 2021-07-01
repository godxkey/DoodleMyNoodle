using System;
using Unity.Entities;
using UnityEngine;


[DisallowMultipleComponent]
public class TileAuth : MonoBehaviour
{
    public enum TileAuthType
    {
        Terrain, // Might not need this here in the future !!
        Ladder, // Might not need this here in the future !!
    }

    [SerializeField] private TileAuthType _type = TileAuthType.Terrain;

    public bool ShouldBeConvertedToTile()
    {
        switch (_type)
        {
            case TileAuthType.Terrain:
            case TileAuthType.Ladder:
                return true;

            default:
                return false;
        }
    }

    public TileFlagComponent GetTileFlags()
    {
        switch (_type)
        {
            case TileAuthType.Terrain:
                return TileFlagComponent.Terrain;

            case TileAuthType.Ladder:
                return TileFlagComponent.Ladder;

            default:
                return default;
        }
    }
}
