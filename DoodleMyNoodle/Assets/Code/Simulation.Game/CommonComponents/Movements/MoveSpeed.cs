using Unity.Entities;

[GenerateAuthoringComponent]
public struct MoveSpeed : IComponentData
{
    public fix Value;
}