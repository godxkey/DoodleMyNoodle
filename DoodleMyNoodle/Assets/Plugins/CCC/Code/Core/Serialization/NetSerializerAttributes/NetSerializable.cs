
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class NetSerializableAttribute : Attribute
{
    public NetSerializableAttribute()
    {
    }

    public bool IsBaseClass;
}


[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class GenerateSerializerAttribute : Attribute
{
    public Type Type;

    public GenerateSerializerAttribute(Type type)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }
}
