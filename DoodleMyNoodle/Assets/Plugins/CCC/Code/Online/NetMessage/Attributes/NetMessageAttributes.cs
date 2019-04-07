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
        if(fieldInfo.GetCustomAttributes(typeof(IgnoreAttribute), false).Length != 0)
        {
            return true;
        }
        else
        {
            if (!fieldInfo.IsPublic)
            {
                Debug.LogWarning("The " + fieldInfo.Name + " field is not public and will be ignored in net serializing." +
                    " If it is intended, please add [NetMessageAttributes.Ignore] to the field.");
                return true;
            }
            return false;
        }
    }
}
