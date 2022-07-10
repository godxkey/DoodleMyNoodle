using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

public static class NativeCollectionExtensions
{
    public static bool AddUnique<T>(this NativeList<T> list, T item) where T : unmanaged, IEquatable<T>
    {
        if (!list.Contains(item))
        {
            list.Add(item);
            return true;
        }
        return false;
    }

    public static int RemoveDuplicates<T>(this NativeList<T> list) where T : unmanaged, IEquatable<T>
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

    public static void SetOrAdd<TKey, TValue>(this NativeParallelHashMap<TKey, TValue> nativeHashMap, TKey key, TValue value)
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

    public static NativeSlice<T> Slice<T>(this NativeList<T> list) where T : unmanaged
    {
        return list.AsArray().Slice();
    }

    public static NativeSlice<T> Slice<T>(this NativeList<T> list, int start) where T : unmanaged
    {
        return list.AsArray().Slice(start);
    }

    public static NativeSlice<T> Slice<T>(this NativeList<T> list, int start, int length) where T : unmanaged
    {
        return list.AsArray().Slice(start, length);
    }

    public static T Last<T>(this NativeList<T> list) where T : unmanaged => list[list.Length - 1];
    public static T Last<T>(this NativeArray<T> array) where T : struct => array[array.Length - 1];
    public static T LastOrDefault<T>(this NativeList<T> list) where T : unmanaged => list.Length > 0 ? list[list.Length - 1] : default;
    public static T LastOrDefault<T>(this NativeArray<T> array) where T : struct => array.Length > 0 ? array[array.Length - 1] : default;

    public static void Swap<T>(this NativeList<T> list, int indexA, int indexB) where T : unmanaged
    {
        var temp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = temp;
    }
    public static void Swap<T>(this NativeArray<T> array, int indexA, int indexB) where T : struct
    {
        var temp = array[indexA];
        array[indexA] = array[indexB];
        array[indexB] = temp;
    }
    public static void Reverse<T>(this NativeArray<T> array) where T : struct
    {
        int halfLength = array.Length / 2;
        int end = array.Length - 1;
        for (int i = 0; i < halfLength; i++)
        {
            array.Swap(i, end - i);
        }
    }
    public static void Reverse<T>(this NativeList<T> list) where T : unmanaged
    {
        int halfLength = list.Length / 2;
        int end = list.Length - 1;
        for (int i = 0; i < halfLength; i++)
        {
            list.Swap(i, end - i);
        }
    }
}
