using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

public static class NativeCollectionExtensions
{
    public static bool AddUnique<T>(this NativeList<T> list, T item) where T : struct, IEquatable<T>
    {
        if (!list.Contains(item))
        {
            list.Add(item);
            return true;
        }
        return false;
    }

    public static int RemoveDuplicates<T>(this NativeList<T> list) where T : struct, IEquatable<T>
    {
        int removeCount = 0;
        for (int i = 0; i < list.Length; i++)
        {
            T current = list[i];

            for (int r = list.Length - 1; r > i; r--)
            {
                if (current.Equals(list[r]))
                {
                    list.RemoveAt(r);
                    removeCount++;
                }
            }
        }

        return removeCount;
    }

    public static void SetOrAdd<TKey, TValue>(this NativeHashMap<TKey, TValue> nativeHashMap, TKey key, TValue value)
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

    public static NativeSlice<T> Slice<T>(this NativeList<T> list) where T : struct
    {
        return list.AsArray().Slice();
    }

    public static NativeSlice<T> Slice<T>(this NativeList<T> list, int start) where T : struct
    {
        return list.AsArray().Slice(start);
    }

    public static NativeSlice<T> Slice<T>(this NativeList<T> list, int start, int length) where T : struct
    {
        return list.AsArray().Slice(start, length);
    }
}
