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
        for (int i = 0; i < entities.Count; i++)
        {
            T comp = entities[i].GetComponent<T>();
            if (comp != null)
                action(comp);
        }
    }
    internal void ForEveryEntityWithComponent<T1, T2>(Action<T1, T2> action)
    {
        var entities = SimModules.world.entities;
        for (int i = 0; i < entities.Count; i++)
        {
            T1 comp1 = entities[i].GetComponent<T1>();
            if (comp1 == null)
                continue;
            T2 comp2 = entities[i].GetComponent<T2>();
            if (comp1 == null)
                continue;
            action(comp1, comp2);
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