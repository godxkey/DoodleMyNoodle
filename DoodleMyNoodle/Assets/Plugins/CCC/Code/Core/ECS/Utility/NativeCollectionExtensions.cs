using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

public static class NativeCollectionExtensions
{
    public static void SetOrAdd<TKey, TValue>(this NativeHashMap<TKey, TValue> nativeHashMap, in TKey key, in TValue value)
        where TKey : struct, IEquatable<TKey>
        where TValue : struct
    {
        if (nativeHashMap.ContainsKey(key))
        {
            nativeHashMap[key] = value;
        }
        else
        {
            nativeHashMap.Add(key, value);
        }
    }
    //public static bool Contains<T>(this NativeArray<T> array, in T value)
    //    where T : struct
    //{
    //    return array.IndexOf(value) != -1;
    //}
}
