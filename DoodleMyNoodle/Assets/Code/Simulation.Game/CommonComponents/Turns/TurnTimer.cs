using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct TurnTimer : IComponentData
{
    public fix Value;
}
