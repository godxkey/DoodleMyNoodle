using Unity.Entities;

public struct AirControl : IComponentData
{
    public fix Value;

    public static implicit operator fix(AirControl val) => val.Value;
    public static implicit operator AirControl(fix val) => new AirControl() { Value = val };
}