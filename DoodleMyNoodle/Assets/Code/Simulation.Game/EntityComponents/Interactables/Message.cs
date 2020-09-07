using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct Message : IComponentData
{
    public NativeString64 Value;

    public override string ToString()
    {
        return Value.ToString();
    }
}