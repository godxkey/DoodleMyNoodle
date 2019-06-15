using System.Collections;
using System.Collections.Generic;

public static class SimWorldExtensions
{
    public static SimEntity FindEntityWithName(this SimWorld world, string name)
    {
        var entities = world.entities;
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i].name == name)
                return entities[i];
        }
        return null;
    }

    public static SimEntity FindEntityWithComponent<T>(this SimWorld world) where T : SimComponent
    {
        var entities = world.entities;
        for (int i = 0; i < entities.Count; i++)
        {
            T comp = entities[i].GetComponent<T>();
            if (comp != null)
                return entities[i];
        }
        return null;
    }

    public static SimEntity FindEntityWithComponent<T>(this SimWorld world, out T comp) where T : SimComponent
    {
        var entities = world.entities;
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
