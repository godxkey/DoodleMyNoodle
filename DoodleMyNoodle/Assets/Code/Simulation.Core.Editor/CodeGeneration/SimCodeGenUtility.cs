using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class SimCodeGenUtility
{
    public static List<Type> GetSimComponentTypesToGenerateViewFor()
    {
        List<Type> componentTypes = new List<Type>();

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (SimCodeGenSettings.targetAssemblies.ContainsKey(assembly.GetName().Name) == false)
                continue;

            foreach (Type type in assembly.GetTypes())
            {
                if (ShouldGenerateViewForComponentType(type))
                {
                    componentTypes.Add(type);
                }
            }
        }

        return componentTypes;
    }

    public static FieldInfo[] GetSimComponentViewFieldsToGenerate(Type simComponentType)
    {
        return simComponentType
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(ShouldGenerateViewForField)
            .ToArray();
    }

    public static bool ShouldGenerateViewForComponentType(Type type)
    {
        return type.IsSubclassOf(typeof(SimComponent))
            && type.GetCustomAttribute<SimViewGenerationIgnoreAttribute>() == null;
    }

    public static bool ShouldGenerateViewForField(FieldInfo field)
    {
        return field.GetCustomAttribute<SimViewGenerationIgnoreAttribute>() == null
            && field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) == false
            && field.FieldType.IsSubclassOf(typeof(SimObject)) == false;
    }


    public static string GetSimComponentViewName(Type simComponentType)
    {
        return simComponentType.Name + "View";
    }
}
