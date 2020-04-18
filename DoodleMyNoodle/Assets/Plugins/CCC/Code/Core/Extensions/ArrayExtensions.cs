using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static  class ArrayExtensions
{
    public static bool IsValidIndex(this Array array, int index)
    {
        return index >= 0 && index < array.Length;
    }

    public static bool IsValidIndex(this Array array, uint index)
    {
        return index < array.Length;
    }

    public static T TryGetAt<T>(this T[] array, uint index)
    {
        array.TryGetAt(index, out T result);
        return result;
    }
    public static T TryGetAt<T>(this T[] array, int index)
    {
        array.TryGetAt(index, out T result);
        return result;
    }
    public static bool TryGetAt<T>(this T[] array, uint index, out T result)
    {
        if (array.IsValidIndex(index))
        {
            result = array[index];
            return true;
        }
        result = default;
        return false;
    }
    public static bool TryGetAt<T>(this T[] array, int index, out T result)
    {
        if (array.IsValidIndex(index))
        {
            result = array[index];
            return true;
        }
        result = default;
        return false;
    }

    public static bool Contains(this Array array, object obj)
    {
        for (int i = 0; i < array.Length; i++)
            if (array.GetValue(i).Equals(obj))
                return true;
        return false;
    }

    public static bool ContainsNull(this Array array)
    {
        for (int i = 0; i < array.Length; i++)
            if (array.GetValue(i) == null)
                return true;
        return false;
    }

    public static bool Contains<T>(this T[] array, Predicate<T> predicate)
    {
        for (int i = 0; i < array.Length; i++)
            if (predicate(array[i]))
                return true;
        return false;
    }

    public static bool Contains<T>(this T[] array, T element)
    {
        for (int i = 0; i < array.Length; i++)
            if (EqualityComparer<T>.Default.Equals(array[i], element))
                return true;
        return false;
    }

    public static bool Contains<T>(this Array array)
    {
        for (int i = 0; i < array.Length; i++)
            if (array.GetValue(i) is T)
                return true;
        return false;
    }

    public static T Find<T>(this T[] array, Predicate<T> predicate)
    {
        for (int i = 0; i < array.Length; i++)
            if (predicate(array[i]))
                return array[i];
        return default;
    }
    public static T Find<T>(this Array array)
    {
        for (int i = 0; i < array.Length; i++)
            if (array.GetValue(i) is T)
                return (T)array.GetValue(i);
        return default;
    }

    public static T Last<T>(this T[] list)
    {
        return list[list.Length - 1];
    }
    public static int LastIndex(this Array list)
    {
        return list.Length - 1;
    }
    public static int CountOf<T>(this T[] list, T element)
    {
        int amount = 0;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].Equals(element))
                amount++;
        }
        return amount;
    }

    public static T PickRandom<T>(this T[] list)
    {
        if (list.Length == 0)
            return default(T);
        if (list.Length == 1)
            return list[0];
        return list[UnityEngine.Random.Range(0, list.Length)];
    }

    public static void Shuffle<T>(this T[] list)
    {
        int chosen = -1;
        T temp;
        for (int i = list.Length - 1; i >= 1; i--)
        {
            chosen = UnityEngine.Random.Range(0, i + 1);
            if (chosen == i)
                continue;

            temp = list[chosen];
            list[chosen] = list[i];
            list[i] = temp;
        }
    }
}
