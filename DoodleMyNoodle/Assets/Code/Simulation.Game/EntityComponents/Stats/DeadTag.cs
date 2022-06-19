using Unity.Entities;
using Unity.Mathematics;

public struct DeadTag : IComponentData
{
    
}

public struct DeadTimestamp : IComponentData
{
    public fix TimeOfDeath;
}