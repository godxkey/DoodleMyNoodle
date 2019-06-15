using System.Collections;
using System.Collections.Generic;

public static class SimEntityExtensions
{
    public static T GetComponent<T>(this SimEntity entity) where T : SimComponent
    {
        var components = entity.components;
        for (int i = 0; i < components.Count; i++)
        {
            if (components[i] is T)
                return (T)components[i];
        }

        return default;
    }
}
