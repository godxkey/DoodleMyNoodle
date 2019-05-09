using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class NetMessageCodeGenUtility
{
    public static ReadOnlyCollection<Type> GetNetMessageTypes()
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
                if (type.GetCustomAttribute<NetMessageAttribute>() != null)
                {
                    if (!type.IsClass || type.BaseType == typeof(object))
                    {
                        netMessageTypes.Add(type);
                    }
                    else
                    {
                        Debug.LogError(type + " ERROR: Net message classes cannot use inheritance. It is not a supported feature. Consider using composition instead.");
                    }
                }
            }
        }

        return netMessageTypes.GetInternalList();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetSerializerNameFromType(Type type)
    {
        if (type.IsArray)
        {
            return "ArrayNetSerializer_" + type.GetElementType().Name;
        }
        else
        {
            return "NetSerializer_" + type.Name;
        }
    }
}
