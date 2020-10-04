using Unity.Entities;
using Unity.Mathematics;

public struct InventoryCapacity : IComponentData
{
    public int Value;

    public static implicit operator int(InventoryCapacity val) => val.Value;
    public static implicit operator InventoryCapacity(int val) => new InventoryCapacity() { Value = val };
}