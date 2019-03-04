using System;
using System.Collections.Generic;

public class Factory<Key, Value>
{
    public Factory()
    {

    }
    public Factory(ValueTuple<Key, Func<Value>>[] keyValuePairs)
    {
        foreach (var pair in keyValuePairs)
        {
            registry.Add(pair.Item1, pair.Item2);
        }
    }
    public Factory(KeyValuePair<Key, Func<Value>>[] keyValuePairs)
    {
        foreach (var pair in keyValuePairs)
        {
            registry.Add(pair.Key, pair.Value);
        }
    }

    private Dictionary<Key, Func<Value>> registry = new Dictionary<Key, Func<Value>>();

    public void Register<T>(Key key) where T : Value, new()
    {
        registry[key] = () => new T();
    }

    public Value CreateValue(Key type)
    {
        return registry[type].Invoke();
    }
}
