using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct TurnTeamCount : IComponentData
{
    public int Value;
}
