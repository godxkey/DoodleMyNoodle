using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct TurnCurrentTeam : IComponentData
{
    public int Value;
}
