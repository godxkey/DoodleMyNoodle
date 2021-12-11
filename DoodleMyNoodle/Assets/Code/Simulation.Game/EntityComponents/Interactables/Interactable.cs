using Unity.Entities;

public struct InteractableFlag : IComponentData
{
    public bool Value;

    public static implicit operator bool(InteractableFlag val) => val.Value;
    public static implicit operator InteractableFlag(bool val) => new InteractableFlag() { Value = val };
}