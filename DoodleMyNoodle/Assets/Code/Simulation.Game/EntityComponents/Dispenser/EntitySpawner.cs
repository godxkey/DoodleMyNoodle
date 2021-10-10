using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct EntitySpawnerSetting : IComponentData
{
    public bool OnlyWhenSignalOn;

    public fix ShootSpeedMin;
    public fix ShootSpeedMax;

    public fix2 ShootDirectionMin;
    public fix2 ShootDirectionMax;

    public int AmountSpawned;
    public int Quantity;

    public bool SpawnedRandomly;

    public TimeValue SpawnPeriod;
}

public struct EntitySpawnerState : IComponentData
{
    public int IndexToSpawn;
    public int TotalAmountSpawned;
    public TimeValue TrackedTime;
}