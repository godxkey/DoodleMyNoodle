using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class NetMessageAttributes
{
    public static bool ShouldIgnoreCodeGeneration(Type type)
    {
        return type.GetCustomAttributes(typeof(IgnoreAttribute), false).Length != 0;
    }
    public static bool ShouldIgnoreCodeGeneration(FieldInfo fieldInfo)
    {
        return fieldInfo.GetCustomAttributes(typeof(IgnoreAttribute), false).Length != 0;
    }
}
