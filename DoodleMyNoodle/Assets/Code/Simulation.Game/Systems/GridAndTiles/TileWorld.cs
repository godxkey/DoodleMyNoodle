using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.Entities;
using Unity.Collections;
using System;

public struct TileWorld
{
    private GridInfo _gridInfo;
    private NativeArray<GridTileReference> _tileReferences;
    private ComponentDataFromEntity<TileFlagComponent> _tileFlags;
    //private BufferFromEntity<TileActorReference> _tileActorBuffers;

    public TileWorld(GridInfo gridInfo, NativeArray<GridTileReference> tileReferences, ComponentDataFromEntity<TileFlagComponent> tileFlags)
    {
        _gridInfo = gridInfo;
        _tileReferences = tileReferences;
        _tileFlags = tileFlags;
        //_tileActorBuffers = tileActorBuffers;
    }

    public bool IsValid(int2 tilePos)
    {
        return _gridInfo.Contains(tilePos);
    }

    public Entity GetEntity(int2 tilePos)
    {
        if (!IsValid(tilePos))
        {
            return Entity.Null;
        }

        int2 offset = tilePos - _gridInfo.TileMin;
        int index = offset.x + (offset.y * _gridInfo.Width);

        return _tileReferences[index].Tile;
    }

    public TileFlagComponent GetFlags(Entity tileEntity)
    {
        if (tileEntity != Entity.Null)
        {
            return _tileFlags[tileEntity];
        }
        else
        {
            return TileFlagComponent.OutOfGrid;
        }
    }

    public TileFlagComponent GetFlags(int2 tilePos)
    {
        return GetFlags(GetEntity(tilePos));
    }

    // not allowed yet because of a Unity bug that marks the BufferFromEntity as modified when they are readonly ...
    /*
    public DynamicBuffer<TileActorReference> GetTileActors(Entity tileEntity)
    {
        if (tileEntity != Entity.Null)
        {
            return _tileActorBuffers[tileEntity];
        }
        else
        {
            return default;
        }
    }

    public DynamicBuffer<TileActorReference> GetTileActors(int2 tilePos)
    {
        return GetTileActors(GetEntity(tilePos));
    }*/

    public bool CanStandOn(int2 tilePos)
    {
        var flags = GetFlags(tilePos);

        if (flags.IsLadder)
        {
            return true;
        }

        if (flags.IsEmpty)
        {
            var underFlags = GetFlags(tilePos + int2(0, -1));

            if (underFlags.IsTerrain)
            {
                return true;
            }
        }

        return false;
    }
}