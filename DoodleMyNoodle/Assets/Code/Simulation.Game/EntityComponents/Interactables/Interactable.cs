using Unity.Entities;

public struct Interactable : IComponentData
{
    public fix Range;
    public bool OnlyOnce;
    public fix Delay;
}

public struct InteractableFlag : IComponentData
{
    public bool Value;

    public static implicit operator bool(InteractableFlag val) => val.Value;
    public static implicit operator InteractableFlag(bool val) => new InteractableFlag() { Value = val };
}