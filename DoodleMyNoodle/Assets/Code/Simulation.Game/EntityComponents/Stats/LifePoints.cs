using Unity.Entities;

public struct LifePoints : IComponentData
{
    public int Value;

    public static implicit operator int(LifePoints val) => val.Value;
    public static implicit operator LifePoints(int val) => new LifePoints() { Value = val };
}

public struct LifePointLostAction : IBufferElementData
{
    public Entity Value;

    public static implicit operator Entity(LifePointLostAction val) => val.Value;
    public static implicit operator LifePointLostAction(Entity val) => new LifePointLostAction() { Value = val };
}