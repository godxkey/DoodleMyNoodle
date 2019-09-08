﻿using System;
using System.Collections;
using System.Collections.Generic;

public class SimModuleWorldSearcher : IDisposable
{
    internal SimEntity FindEntityWithName(string name)
    {
        var entities = SimModules.World.Entities;
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i].gameObject.name == name)
                return entities[i];
        }
        return null;
    }

    internal SimEntity FindEntityWithComponent<T>()
    {
        var entities = SimModules.World.Entities;
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
        var entities = SimModules.World.Entities;
        for (int i = 0; i < entities.Count; i++)
        {
            comp = entities[i].GetComponent<T>();
            if (comp != null)
                return entities[i];
        }

        comp = default;
        return null;
    }

    public void Dispose()
    {
    }
}