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
        Bedrock, // Might not need this here in the future !!
    }

    public enum ShapeAuthType
    {
        Full,
        CornerTopLeft,
        CornerTopRight,
        CornerBottomLeft,
        CornerBottomRight,
    }

    [SerializeField] private TileAuthType _type = TileAuthType.Terrain;
    [SerializeField] private ShapeAuthType _shape = ShapeAuthType.Full;

    public bool ShouldBeConvertedToTile()
    {
        switch (_type)
        {
            case TileAuthType.Terrain:
            case TileAuthType.Ladder:
            case TileAuthType.Bedrock:
                return true;

            default:
                return false;
        }
    }

    public TileFlagComponent GetTileFlags()
    {
        TileFlagComponent flags = default;

        switch (_type)
        {
            case TileAuthType.Terrain:
                flags = TileFlagComponent.Terrain;
                break;

            case TileAuthType.Ladder:
                flags = TileFlagComponent.Ladder;
                break;

            case TileAuthType.Bedrock:
                flags = TileFlagComponent.Bedrock;
                break;
        }

        // reset shape
        flags.Value &= ~(TileFlags.Shape_Full | TileFlags.Shape_CornerAny);
        switch (_shape)
        {
            case ShapeAuthType.Full:
                flags.Value |= TileFlags.Shape_Full;
                break;
            case ShapeAuthType.CornerTopLeft:
                flags.Value |= TileFlags.Shape_CornerTopLeft;
                break;
            case ShapeAuthType.CornerTopRight:
                flags.Value |= TileFlags.Shape_CornerTopRight;
                break;
            case ShapeAuthType.CornerBottomLeft:
                flags.Value |= TileFlags.Shape_CornerBottomLeft;
                break;
            case ShapeAuthType.CornerBottomRight:
                flags.Value |= TileFlags.Shape_CornerBottomRight;
                break;
        }

        return flags;
    }
}
