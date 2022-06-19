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


public struct LevelDefinitionMobSpawn : IBufferElementData
{
    public Entity MobToSpawn;
    public MobSpawmModifierFlags Flags;
    public fix2 Position;
}