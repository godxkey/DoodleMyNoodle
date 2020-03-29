using System;
using Unity.Collections;
using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(MaximumInt<TurnCurrentTeam>))]

[Serializable]
public struct TurnCurrentTeam : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}
