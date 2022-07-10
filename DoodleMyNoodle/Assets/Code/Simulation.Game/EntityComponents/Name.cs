
using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct Name : IComponentData
{
    // MAXIMUM 30 CHARACTERS !!!
    public FixedString64Bytes Value;

    public override string ToString()
    {
        return Value.ToString();
    }
}
