using Unity.Entities;

public struct GameActionThrowSettings : IComponentData
{
    public Entity ProjectilePrefab;
    public fix SpeedMax;
    public fix SpeedMin;
}