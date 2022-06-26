using Unity.Entities;

public struct XP : IComponentData
{
    public fix Value;

    public static implicit operator fix(XP val) => val.Value;
    public static implicit operator XP(fix val) => new XP() { Value = val };
}