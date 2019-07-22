using System;

public class SimModuleWorldSearcher
{
    internal SimEntity FindEntityWithName(string name)
    {
        var entities = SimModules.world.entities;
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i].gameObject.name == name)
                return entities[i];
        }
        return null;
    }

    internal SimEntity FindEntityWithComponent<T>()
    {
        var entities = SimModules.world.entities;
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
        var entities = SimModules.world.entities;
        for (int i = 0; i < entities.Count; i++)
        {
            comp = entities[i].GetComponent<T>();
            if (comp != null)
                return entities[i];
        }

        comp = default;
        return null;
    }

    internal void ForEveryEntityWithComponent<T>(Action<T> action)
    {
        var entities = SimModules.world.entities;
        T comp = default;
        for (int i = 0; i < entities.Count; i++)
        {
            comp = entities[i].GetComponent<T>();
            if (comp != null)
                action(comp);
        }
    }

    /// <summary>
    /// Return false to stop the iteration
    /// </summary>
    internal void ForEveryEntityWithComponent<T>(Func<T, bool> action)
    {
        var entities = SimModules.world.entities;
        T comp = default;
        for (int i = 0; i < entities.Count; i++)
        {
            comp = entities[i].GetComponent<T>();
            if (comp != null)
            {
                if (!action(comp))
                {
                    return;
                }
            }
        }
    }
}