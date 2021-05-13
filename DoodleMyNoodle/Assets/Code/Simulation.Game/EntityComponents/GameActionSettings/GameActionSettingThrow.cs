using Unity.Entities;

public struct GameActionSettingThrow : IComponentData
{
    public Entity ProjectilePrefab;
    public fix SpeedMax;
    public fix SpeedMin;
}