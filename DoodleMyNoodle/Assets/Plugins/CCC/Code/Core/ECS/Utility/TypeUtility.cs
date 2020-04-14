using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

public static class TypeUtility
{
    public static IEnumerable<Type> GetECSTypesDerivedFrom(Type type)
    {
#if UNITY_EDITOR
        return UnityEditor.TypeCache.GetTypesDerivedFrom(type);
#else
        var types = new List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!TypeManager.IsAssemblyReferencingEntities(assembly))
                continue;

            try
            {
                var assemblyTypes = assembly.GetTypes();
                foreach (var t in assemblyTypes)
                {
                    if (type != t && type.IsAssignableFrom(t))
                        types.Add(t);
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                foreach (var t in e.Types)
                {
                    if (t != null && type != t && type.IsAssignableFrom(t))
                        types.Add(t);
                }

                Debug.LogWarning($"Failed loading assembly: {(assembly.IsDynamic ? assembly.ToString() : assembly.Location)}");
            }
        }

        return types;
#endif
    }

    public static IEnumerable<Type> GetTypesDerivedFrom(Type baseType)
    {
#if UNITY_EDITOR
        return UnityEditor.TypeCache.GetTypesDerivedFrom(baseType);
#else

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Assembly baseTypeAssembly = baseType.Assembly;
        string assemblyName = baseTypeAssembly.GetName().Name;

        return

            // Join results
            Concat(assemblies

                // Assemblies that reference Simulation.Game (or Simulation.Game itself)
                .Where(assembly => assembly == baseTypeAssembly || assembly.GetReferencedAssemblies().Contains(x => x.Name == assemblyName))

                    // Get types in assembly
                    .Select(assembly => assembly.GetTypes()

                        // Get all types that inherit from GameActionA
                        .Where(type => baseType.IsAssignableFrom(type) && type != baseType)));
#endif
    }


    private static IEnumerable<T> Concat<T>(IEnumerable<IEnumerable<T>> sequences)
    {
        return sequences.SelectMany(x => x);
    }
}
