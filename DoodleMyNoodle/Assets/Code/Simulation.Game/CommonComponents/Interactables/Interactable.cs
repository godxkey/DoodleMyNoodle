using Unity.Entities;

public struct Interactable : IComponentData
{
    public float Range;
    public bool OnlyOnce;
    public float Delay;
}