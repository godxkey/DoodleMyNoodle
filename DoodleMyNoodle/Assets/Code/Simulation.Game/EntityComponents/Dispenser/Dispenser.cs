using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct Dispenser : IComponentData
{
    public bool OnlyWhenSignalOn;

    public fix ShootSpeedMin;
    public fix ShootSpeedMax;

    public fix2 ShootDirectionMin;
    public fix2 ShootDirectionMax;

    public int AmountSpawned;
    public int Quantity;
    public int IndexToSpawn;
    public int TotalAmountSpawned;

    public bool SpawnedRandomly;
}