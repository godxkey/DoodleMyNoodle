using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Interactable : IComponentData
{
    public bool OnlyOnce;
}