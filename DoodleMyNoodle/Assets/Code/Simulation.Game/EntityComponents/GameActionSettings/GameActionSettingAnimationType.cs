using Unity.Entities;
using Unity.Mathematics;

public struct GameActionSettingAnimationType : IComponentData
{
    public int AnimationType;
    public fix Duration;
}