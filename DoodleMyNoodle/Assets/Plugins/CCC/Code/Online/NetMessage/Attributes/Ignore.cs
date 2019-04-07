using System;

public partial class NetMessageAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreAttribute : Attribute
    {
    }
}
