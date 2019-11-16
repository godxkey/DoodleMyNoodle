using System;
using System.Collections;
using System.Collections.Generic;

internal class SimModuleWorldSearcher : SimModuleBase
{
    internal SimEntity FindEntityWithName(string name)
    {
        var entities = SimModules._World.Entities;
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i].gameObject.name == name)
                return entities[i];
        }
        return null;
    }

    internal SimEntity FindEntityWithComponent<T>()
    {
        var entities = SimModules._World.Entities;
        for (int i = 0; i < entities.Count; i++)
        {
            T comp = entities[i].GetComponent<T>();
            if (comp != null)
                return entities[i];
        }
        return null;
    }

    internal SimEntity FindEntityWithComponent<T>(out T comp)
    {
        var entities = SimModules._World.Entities;
        for (int i = 0; i < entities.Count; i++)
        {
            comp = entities[i].GetComponent<T>();
            if (comp != null)
                return entities[i];
        }

        comp = default;
        return null;
    }
}