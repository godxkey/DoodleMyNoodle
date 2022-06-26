using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using System;

[Flags]
public enum MobSpawmModifierFlags
{
    None = 0,
    Armored = 1 << 0,
    Brutal = 1 << 1,
    Fast = 1 << 2,
    Explosive = 1 << 3,
}

public struct LevelDefinitionTag : IComponentData { }

/// <summary>
/// A level with no combat. Generally to buy items.
/// </summary>
public struct LevelDefinitionPitStopTag : IComponentData { }

/// <summary>
/// Spawns for mobs
/// </summary>
public struct LevelDefinitionMobSpawn : IBufferElementData
{
    public Entity MobToSpawn;
    public MobSpawmModifierFlags Flags;
    public fix2 Position;
}

/// <summary>
/// A time limit to the current level
/// </summary>
public struct LevelDefinitionDuration : IComponentData
{
    public fix Value;

    public static implicit operator fix(LevelDefinitionDuration val) => val.Value;
    public static implicit operator LevelDefinitionDuration(fix val) => new LevelDefinitionDuration() { Value = val };
}