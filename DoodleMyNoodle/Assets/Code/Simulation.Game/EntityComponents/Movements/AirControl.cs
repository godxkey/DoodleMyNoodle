using Unity.Entities;

public struct AirControl : IComponentData
{
    public fix Value;

    public static implicit operator fix(AirControl val) => val.Value;
    public static implicit operator AirControl(fix val) => new AirControl() { Value = val };
}

public struct NonAirControlFriction : IComponentData
{
    public fix Value;

    public static implicit operator fix(NonAirControlFriction val) => val.Value;
    public static implicit operator NonAirControlFriction(fix val) => new NonAirControlFriction() { Value = val };
}