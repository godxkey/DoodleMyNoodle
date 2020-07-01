using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngineX;

public static class NetSerializationCodeGenUtility
{
    public static string GetSerializerNameFromType(Type type)
    {
        if (type.IsArray)
        {
            return "ArrayNetSerializer_" + type.GetElementType().GetNiceFullNameWithUnderscores();
        }
        else if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            return "ListNetSerializer_" + type.GetGenericArguments()[0].GetNiceFullNameWithUnderscores();
        }
        else if (type.IsEnum)
        {
            return "StaticNetSerializer_" + type.GetEnumUnderlyingType().GetNiceFullNameWithUnderscores();
        }
        else
        {
            return "StaticNetSerializer_" + type.GetNiceFullNameWithUnderscores();
        }
    }

    public static string GetNiceFullNameWithUnderscores(this Type type)
    {
        string result = type.GetPrettyFullName();

        if (result.Contains("+"))
            result = result.Replace('+', '_');

        if (result.Contains("."))
            result = result.Replace('.', '_');

        return result;
    }

    public static List<Type> GetNetSerializableTypes()
    {
        Func<Type, Type, int> typeComparer = (t1, t2) =>
        {
            return string.Compare(t1.FullName, t2.FullName);
        };

        ManualSortedList<Type> netMessageTypes = new ManualSortedList<Type>(typeComparer, 128);

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttribute<NetSerializableAttribute>() != null)
                {
                    netMessageTypes.Add(type);
                }
            }
        }

        return netMessageTypes.internalList;
    }

    public static bool ConsideredAsValueType(Type type)
    {
        return !type.IsClass || type == typeof(string) || type.IsArray;
    }

    public static bool ShouldIgnoreCodeGeneration(Type type)
    {
        return type.GetCustomAttributes(typeof(NotNetSerializedAttribute), false).Length != 0;
    }
    public static bool ShouldIgnoreCodeGeneration(FieldInfo fieldInfo)
    {
        if (fieldInfo.GetCustomAttributes(typeof(NotNetSerializedAttribute), false).Length != 0)
        {
            return true;
        }
        else
        {
            if (!fieldInfo.IsPublic)
            {
                Debug.LogWarning("The " + fieldInfo.Name + " field is not public and will be ignored in net serialization." +
                    " If it is intended, please add [NotNetSerialized] to the field.");
                return true;
            }
            return false;
        }
    }
}
