using System;
using System.Collections.ObjectModel;
using System.Reflection;

public static class NetMessageCodeGenerationUtility
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
                if (typeof(INetSerializable).IsAssignableFrom(type)
                    && !type.IsAbstract
                    && !type.IsInterface)
                {
                    netMessageTypes.Add(type);
                }
            }
        }

        return netMessageTypes.GetInternalList();
    }
}
