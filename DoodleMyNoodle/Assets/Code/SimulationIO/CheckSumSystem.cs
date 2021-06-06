using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using SimulationControl;
using CCC.Fix2D;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngineX;

[DisableAutoCreation]
[UpdateBefore(typeof(ConstructSimulationTickSystem))]
public class RequestChecksumSystem : SystemBase
{
    private ConstructSimulationTickSystem _constructTickSystem;
    private uint _lastChecksumTick = 0;
    private uint _checksumInterval = 60 * 6;

    protected override void OnCreate()
    {
        base.OnCreate();

        _constructTickSystem = World.GetOrCreateSystem<ConstructSimulationTickSystem>();
    }

    protected override void OnUpdate()
    {
        uint nextExpectedConstructedTick = _constructTickSystem.FindExpectedNewTickId();
        if (nextExpectedConstructedTick > _lastChecksumTick + _checksumInterval)
        {
            _constructTickSystem.RequestChecksumAfterNextTick();
            _lastChecksumTick = nextExpectedConstructedTick;
        }
    }
}

[DisableAutoCreation]
[UpdateAfter(typeof(TickSimulationSystem))]
public class ChecksumSystem : SystemBase
{
    private DirtyValue<uint> _simWorldReplaceVersion;
    private SimulationWorldSystem _simWorldSystem;
    private TickSimulationSystem _tickSystem;
    private EntityQuery _entitiesWithTranslastionQ;

    private ulong? _producedHash;
    private uint _hashTick;

    protected override void OnCreate()
    {
        base.OnCreate();

        _simWorldSystem = World.GetExistingSystem<SimulationWorldSystem>();
        _tickSystem = World.GetExistingSystem<TickSimulationSystem>();
        _tickSystem.SimulationTicked += OnSimTick;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _tickSystem.SimulationTicked -= OnSimTick;
    }

    private void OnSimTick(SimTickData obj)
    {
        if (obj.ChecksumAfter)
        {
            _producedHash = CreateChecksum();
            _hashTick = obj.ExpectedNewTickId - 1;
        }
    }

    protected override void OnUpdate()
    {
        if (_producedHash != null)
        {
            string str = $"tick {_hashTick}, sim checksum: {_producedHash.Value}";
            DebugScreenMessage.DisplayMessage(str);
            Log.Info(str);
            _producedHash = null;
        }
    }

    private void UpdateSimEntityQuery()
    {
        _simWorldReplaceVersion.Set(_simWorldSystem.ReplaceVersion);
        if (_simWorldReplaceVersion.ClearDirty()) // world replaced!
        {
            // update cached sim queries since world got replaced
            // NB: No need to dispose of these queries because the world gets disposed ...
            _entitiesWithTranslastionQ = _simWorldSystem.SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<FixTranslation>());
        }
    }

    public ulong CreateChecksum()
    {
        UpdateSimEntityQuery();

        NativeArray<FixTranslation> translations = _entitiesWithTranslastionQ.ToComponentDataArray<FixTranslation>(Allocator.TempJob);

        var hashJob = new HasDataJob()
        {
            Hash = new NativeArray<ulong>(1, Allocator.TempJob),
            Translations = translations
        };

        hashJob.Run();
        
        ulong hash = hashJob.Hash[0];
        
        translations.Dispose();
        hashJob.Hash.Dispose();
        
        return hash;
    }

    //[BurstCompile]
    //public struct CollectDataJob : IJobChunk
    //{
    //    // Read-only data in the current chunk
    //    [ReadOnly]
    //    public EntityTypeHandle EntityTypeHandleAccessor;

    //    [ReadOnly]
    //    public ComponentTypeHandle<FixTranslation> TranslationTypeHandleAccessor;

    //    //[NativeDisableContainerSafetyRestriction]
    //    //[NativeDisableParallelForRestriction]
    //    public NativeHashMap<Entity, FixTranslation> Map;

    //    public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
    //    {
    //        NativeArray<Entity> entities = chunk.GetNativeArray(EntityTypeHandleAccessor);
    //        NativeArray<FixTranslation> translations = chunk.GetNativeArray(TranslationTypeHandleAccessor);
    //        for (int i = 0; i < entities.Length; i++)
    //        {
    //            Map[entities[i]] = translations[i];
    //        }
    //    }
    //}

    [BurstCompile]
    public struct HasDataJob : IJob
    {
        [ReadOnly]
        public NativeArray<FixTranslation> Translations;

        public NativeArray<ulong> Hash;

        public void Execute()
        {
            unsafe
            {
                var ptr = Translations.GetUnsafeReadOnlyPtr();
                var lengthByte = FixTranslation.BYTE_SIZE * Translations.Length;
                Hash[0] = Crc64.Compute(ptr, lengthByte);
            }
        }
    }
}