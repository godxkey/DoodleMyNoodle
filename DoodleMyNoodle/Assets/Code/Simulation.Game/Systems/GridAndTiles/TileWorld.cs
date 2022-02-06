using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.Entities;
using Unity.Collections;
using System;
using UnityEngineX;
using System.Linq;

public struct TileWorld
{
    private GridInfo _gridInfo;
    private NativeArray<GridTileReference> _tileReferences;
    private ComponentDataFromEntity<TileFlagComponent> _tileFlags;
    //private BufferFromEntity<TileActorReference> _tileActorBuffers;

    public bool IsCreated => _tileReferences.IsCreated;

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

[UpdateAfter(typeof(CreateGridSystem))]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class CreateTileWorldSystem : SimGameSystemBase
{
    private DirtyValue<uint> _gridInfoVersion;
    private NativeArray<GridTileReference> _tileReferencesBuffer;

    public GridInfo GridInfo;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_tileReferencesBuffer.IsCreated)
        {
            _tileReferencesBuffer.Dispose();
        }
    }

    public TileWorld GetTileWorld()
    {
        return new TileWorld(
            GetSingleton<GridInfo>(),
            _tileReferencesBuffer,
            GetComponentDataFromEntity<TileFlagComponent>());
    }

    protected override void OnUpdate()
    {
        var singleton = GetSingletonEntity<GridInfo>();
        
        _gridInfoVersion.Set(EntityManager.GetChunk(singleton).GetComponentVersion(typeof(GridInfo)));

        if (_gridInfoVersion.ClearDirty())
        {
            GridInfo = EntityManager.GetComponentData<GridInfo>(singleton);

            if (_tileReferencesBuffer.IsCreated)
            {
                _tileReferencesBuffer.Dispose();
            }

            _tileReferencesBuffer = EntityManager.GetBufferReadOnly<GridTileReference>(GetSingletonEntity<GridInfo>()).ToNativeArray(Allocator.Persistent);
        }
    }
}

public partial class CommonReads
{
    public static TileWorld GetTileWorld(ISimWorldReadAccessor accessor)
    {
        return accessor.GetExistingSystem<CreateTileWorldSystem>().GetTileWorld();
    }

    public static bool IsTileValid(ISimWorldReadAccessor accessor, int2 tile)
    {
        return accessor.GetExistingSystem<CreateTileWorldSystem>().GridInfo.Contains(tile);
    }
}