using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngineX;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AllowMultipleAttribute : Attribute
{

}

#if DEBUG
public class ReportDuplicateSystemsSystem : ComponentSystem
{
    List<Type> _exclude = new List<Type>();
    List<Type> _allSystemTypes = new List<Type>();

    protected override void OnUpdate()
    {
        GatherAllSystemTypes();

        _exclude.Clear();
        for (int i = 0; i < _allSystemTypes.Count; i++)
        {
            var type = _allSystemTypes[i];
            if (_exclude.Contains(type))
                continue;

            int count = CountOfSystem(type);
            if (count > 1 && !Attribute.IsDefined(type, typeof(AllowMultipleAttribute)))
            {
                _exclude.Add(type);

                Log.Warning($"There are {count} instances of {type} in the {World.Name} world." +
                    $" If this is intended, add the [AllowMultiple] attribute to your system class.");
            }
        }
    }

    private void GatherAllSystemTypes()
    {
        _allSystemTypes.Clear();
        foreach (var sys in World.Systems)
        {
            _allSystemTypes.Add(sys.GetType());
        }
    }

    private int CountOfSystem(Type type)
    {
        int count = 0;
        for (int i = 0; i < _allSystemTypes.Count; i++)
        {
            if (_allSystemTypes[i] == type)
                count++;
        }
        return count;
    }
}
#endif
