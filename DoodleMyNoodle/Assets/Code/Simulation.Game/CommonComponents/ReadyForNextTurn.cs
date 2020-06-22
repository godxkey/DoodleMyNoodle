using System;
using Unity.Entities;

[Serializable]
public struct ReadyForNextTurn : IComponentData
{
    public bool Value;
}
