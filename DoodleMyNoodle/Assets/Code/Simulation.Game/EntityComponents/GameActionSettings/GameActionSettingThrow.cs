using Unity.Entities;

public struct GameActionSettingThrow : IComponentData
{
    public fix SpeedMax;
    public fix SpeedMin;
    public fix SpawnExtraDistance;
}