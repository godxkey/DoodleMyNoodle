using System.Collections;
using Unity.Entities;

public struct BindedSimEntity : IComponentData
{
    public Entity SimEntity;

    public static implicit operator Entity(BindedSimEntity val) => val.SimEntity;
    public static implicit operator BindedSimEntity(Entity val) => new BindedSimEntity() { SimEntity = val };
}
