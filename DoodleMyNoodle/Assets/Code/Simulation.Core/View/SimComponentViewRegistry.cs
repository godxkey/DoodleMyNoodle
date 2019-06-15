using System;
using System.Collections.Generic;

public static class SimComponentViewRegistry
{
    static Dictionary<Type, Type> m_componentToViewTypeMapping = new Dictionary<Type, Type>();

    public static Type GetViewTypeForComponent(Type simComponentType)
    {
        return m_componentToViewTypeMapping[simComponentType];
    }

    public static void RegisterType(Type componentType, Type viewType)
    {
        m_componentToViewTypeMapping[componentType] = viewType;
    }
}
