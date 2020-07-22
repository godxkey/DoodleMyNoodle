
using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct Name : IComponentData
{
    // MAXIMUM 30 CHARACTERS !!!
    public NativeString64 Value;
}
