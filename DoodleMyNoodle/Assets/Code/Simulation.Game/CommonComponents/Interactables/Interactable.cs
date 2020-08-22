using Unity.Entities;

public struct Interactable : IComponentData
{
    public fix Range;
    public bool OnlyOnce;
    public fix Delay;
}