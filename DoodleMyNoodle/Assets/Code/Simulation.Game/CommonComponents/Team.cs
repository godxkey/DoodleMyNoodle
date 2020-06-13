using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct Team : IComponentData
{
    public int Value;
}
