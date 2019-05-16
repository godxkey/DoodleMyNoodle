
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class NetSerializableAttribute : Attribute
{
    public NetSerializableAttribute()
    {
    }

    public bool isBaseClass;
}