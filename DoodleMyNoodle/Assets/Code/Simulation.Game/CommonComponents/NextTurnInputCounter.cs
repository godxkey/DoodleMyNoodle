using System;
using Unity.Entities;

[Serializable]
public struct NextTurnInputCounter : IComponentData
{
    public int Value;
}
