using Unity.Entities;

[GenerateAuthoringComponent]
public struct MoveSpeed : IComponentData
{
    public Fix64 Value;
}