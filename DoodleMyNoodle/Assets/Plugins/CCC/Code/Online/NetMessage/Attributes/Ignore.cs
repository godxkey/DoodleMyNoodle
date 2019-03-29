using System;

public partial class NetMessageAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
    public class IgnoreAttribute : Attribute
    {
    }
}
