using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using UnityEngineX;

public static class ChangeDetection
{
    private static Queue<ChunkTrace> s_chunkTracePool = new Queue<ChunkTrace>();

    public enum LogMode
    {
        NoLog,
        Info,
        Warning,
        Error
    }

    public static LogMode LoggingMode = LogMode.Error;

    public class EntityManagerTrace
    {
        public uint GlobalSystemVersion;

        public List<ChunkTrace> Chunks = new List<ChunkTrace>();
    }

    public class ChunkTrace
    {
        /// <summary>
        /// Every time you perform a structural change affecting a chunk, 
        /// the ECS framework updates the order version of the chunk to the current GlobalSystemVersion value.
        /// <para/>
        /// Structural changes include adding and removing entities, adding or removing the component of an entity,
        /// and changing the value of a shared component (except when you change the value for all entities in a chunk at the same time).
        /// </summary>
        public uint OrderVersion;

        public int EntityCount;

        public List<ComponentTrace> ComponentTraces = new List<ComponentTrace>();

        public bool StructurallyEquals(ChunkTrace other)
        {
            if (other == null)
                return false;

            if (other.OrderVersion != OrderVersion)
                return false;

            if (other.ComponentTraces.Count != ComponentTraces.Count)
                return false;

            for (int i = 0; i < ComponentTraces.Count; i++)
            {
                if (other.ComponentTraces[i].ComponentType != ComponentTraces[i].ComponentType)
                    return false;
            }

            return true;
        }

        public bool ArchetypeEquals(ChunkTrace other)
        {
            if (other == null)
                return false;

            if (other.ComponentTraces.Count != ComponentTraces.Count)
                return false;

            for (int i = 0; i < ComponentTraces.Count; i++)
            {
                if (other.ComponentTraces[i].ComponentType != ComponentTraces[i].ComponentType)
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"V:{OrderVersion} C:{ComponentTraces.Count} E:{EntityCount}";
        }
    }

    public struct ComponentTrace
    {
        public ComponentType ComponentType;
        public uint Version;

        public override string ToString()
        {
            return $"{ComponentType}:{Version}";
        }
    }

    public static void RecordEntityTrace(EntityManager entityManager, EntityManagerTrace trace)
    {
        int chunkI = 0;

        trace.GlobalSystemVersion = entityManager.GlobalSystemVersion;

        using (NativeArray<ArchetypeChunk> chunks = entityManager.GetAllChunks(Allocator.TempJob)) // needs to be temp job to preven unity error :(
        {
            foreach (ArchetypeChunk chunk in chunks)
            {
                while (chunkI >= trace.Chunks.Count)
                    trace.Chunks.Add(s_chunkTracePool.Count > 0 ? s_chunkTracePool.Dequeue() : new ChunkTrace());

                ChunkTrace chunkTrace = trace.Chunks[chunkI++];

                chunkTrace.OrderVersion = chunk.GetOrderVersion();
                chunkTrace.EntityCount = chunk.ChunkEntityCount;
                chunkTrace.ComponentTraces.Clear();

                foreach (ComponentType componentType in chunk.Archetype.GetComponentTypes(Allocator.Temp))
                {
                    chunkTrace.ComponentTraces.Add(new ComponentTrace()
                    {
                        ComponentType = componentType,
                        Version = chunk.GetComponentVersion(componentType)
                    });
                }
            }
        }

        int toRemove = trace.Chunks.Count - chunkI;
        for (int i = 0; i < toRemove; i++)
        {
            s_chunkTracePool.Enqueue(trace.Chunks.Pop());
        }
    }

    public static void CompareAndLogChanges(EntityManagerTrace a, EntityManagerTrace b)
    {
        List<ChunkTrace> chunksA = ListPool<ChunkTrace>.Take();
        List<ChunkTrace> chunksB = ListPool<ChunkTrace>.Take();
        chunksA.AddRange(a.Chunks);
        chunksB.AddRange(b.Chunks);

        ComparisonUtiliy.CompareCummulatedEntities(chunksA, chunksB, a.GlobalSystemVersion);
        ComparisonUtiliy.CompareComponentVersions(chunksA, chunksB);

        ListPool<ChunkTrace>.Release(chunksA);
        ListPool<ChunkTrace>.Release(chunksB);
    }

    private class ComparisonUtiliy
    {
        public static ChunkTrace RemoveChunk(List<ChunkTrace> chunks, ref int iterator, int i)
        {
            ChunkTrace chunkTrace = chunks[i];

            chunks.RemoveAt(i);

            if (i < iterator)
            {
                iterator--;
            }

            return chunkTrace;
        }

        public static int CummulateEntitiesAndRemoveChunksWithIdenticalArchetype(List<ChunkTrace> chunks, ref int iterator, ChunkTrace chunk)
        {
            int entityCount = 0;
            for (int i = chunks.Count - 1; i >= 0; i--)
            {
                if (chunks[i].ArchetypeEquals(chunk))
                {
                    entityCount += RemoveChunk(chunks, ref iterator, i).EntityCount;
                }
            }
            return entityCount;
        }

        public static void CompareCummulatedEntities(List<ChunkTrace> oldChunk, List<ChunkTrace> newChunks, uint globalSystemVersionA)
        {
            int _ = 0;
            for (int i = newChunks.Count - 1; i >= 0; i--)
            {
                if (newChunks[i].OrderVersion > globalSystemVersionA)
                {
                    ChunkTrace changedChunk = newChunks[i];

                    int newEntityCount = CummulateEntitiesAndRemoveChunksWithIdenticalArchetype(newChunks, ref i, changedChunk);
                    int oldEntityCount = CummulateEntitiesAndRemoveChunksWithIdenticalArchetype(oldChunk, ref _, changedChunk);

                    if (newEntityCount > oldEntityCount)
                    {
                        if (oldEntityCount == 0)
                            LogNewArchetype(changedChunk);
                        else
                            LogNewEntities(changedChunk);
                    }
                    else
                    {
                        LogDestroyedEntities(changedChunk);
                    }
                }
            }
        }

        public static void CompareComponentVersions(List<ChunkTrace> oldChunks, List<ChunkTrace> newChunks)
        {
            int iOld = oldChunks.Count - 1;
            int iNew = newChunks.Count - 1;
            while (iOld >= 0)
            {
                ChunkTrace oldChunk = oldChunks[iOld];
                ChunkTrace newChunk = iNew < 0 ? null : newChunks[iNew];

                if (!oldChunk.ArchetypeEquals(newChunk))
                {
                    LogDestroyedArchetype(oldChunk);

                    oldChunks.RemoveAt(iOld);

                    iOld--;
                }
                else
                {
                    for (int c = 0; c < oldChunk.ComponentTraces.Count; c++)
                    {
                        if (oldChunk.ComponentTraces[c].Version != newChunk.ComponentTraces[c].Version)
                        {
                            LogModifiedComponent(oldChunk, oldChunk.ComponentTraces[c].ComponentType);
                        }
                    }

                    iOld--;
                    iNew--;
                }
            }
        }

        public static void ArchetypeToString(StringBuilder stringBuilder, ChunkTrace chunk)
        {
            stringBuilder.Append("archetype (");

            bool addComma = false;

            foreach (var c in chunk.ComponentTraces)
            {
                if (addComma)
                    stringBuilder.Append(", ");

                stringBuilder.Append(c.ComponentType.GetManagedType().GetPrettyName());
                addComma = true;
            }

            stringBuilder.Append(")");
        }

        public static void LogModifiedComponent(ChunkTrace chunk, ComponentType type)
        {
            StringBuilder stringBuilder = StringBuilderPool.Take();

            stringBuilder.Append("modified component ");
            stringBuilder.Append(type);
            stringBuilder.Append(" of ");
            ArchetypeToString(stringBuilder, chunk);

            LogChange(stringBuilder.ToString());

            StringBuilderPool.Release(stringBuilder);
        }

        public static void LogDestroyedEntities(ChunkTrace chunk)
        {
            StringBuilder stringBuilder = StringBuilderPool.Take();

            stringBuilder.Append("destroyed entity/entities of ");
            ArchetypeToString(stringBuilder, chunk);

            LogChange(stringBuilder.ToString());

            StringBuilderPool.Release(stringBuilder);
        }

        public static void LogDestroyedArchetype(ChunkTrace chunk)
        {
            StringBuilder stringBuilder = StringBuilderPool.Take();

            stringBuilder.Append("destroyed ");
            ArchetypeToString(stringBuilder, chunk);

            LogChange(stringBuilder.ToString());

            StringBuilderPool.Release(stringBuilder);
        }

        public static void LogNewEntities(ChunkTrace chunk)
        {
            StringBuilder stringBuilder = StringBuilderPool.Take();

            stringBuilder.Append("new entity/entities of ");
            ArchetypeToString(stringBuilder, chunk);

            LogChange(stringBuilder.ToString());

            StringBuilderPool.Release(stringBuilder);
        }

        public static void LogNewArchetype(ChunkTrace chunk)
        {
            StringBuilder stringBuilder = StringBuilderPool.Take();

            stringBuilder.Append("new ");
            ArchetypeToString(stringBuilder, chunk);

            LogChange(stringBuilder.ToString());

            StringBuilderPool.Release(stringBuilder);
        }

        public static void LogChange(string s)
        {
            s = $"Change detected. Cause (speculative): {s}";

            switch (LoggingMode)
            {
                default:
                case LogMode.NoLog:
                    break;
                case LogMode.Info:
                    UnityEngine.Debug.Log(s);
                    break;
                case LogMode.Warning:
                    UnityEngine.Debug.LogWarning(s);
                    break;
                case LogMode.Error:
                    UnityEngine.Debug.LogError(s);
                    break;
            }
        }
    }
}
