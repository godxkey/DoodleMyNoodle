using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static int RemoveFirst<T>(this List<T> list, Predicate<T> predicate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (predicate(list[i]))
            {
                list.RemoveAt(i);
                return i;
            }
        }
        return -1;
    }
    public static int RemoveNulls<T>(this List<T> list)
    {
        return list.RemoveAll((x) => x == null);
    }
    public static void RemoveLast<T>(this List<T> list)
    {
        int index = list.LastIndex();
        if (index >= 0)
        {
            list.RemoveAt(index);
        }
    }
    public static void RemoveWithLastSwap<T>(this List<T> list, int index)
    {
        int lastIndex = list.Count - 1;

        if (index < 0 || index > lastIndex)
        {
            throw new IndexOutOfRangeException();
        }

        if (index != lastIndex)
        {
            list[index] = list[lastIndex];
        }

        list.RemoveAt(lastIndex);
    }

    public static T Last<T>(this List<T> list)
    {
        return list[list.Count - 1];
    }
    public static int LastIndex<T>(this List<T> list)
    {
        return list.Count - 1;
    }

    public static int CountOf<T>(this List<T> list, T element)
    {
        int amount = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(element))
                amount++;
        }
        return amount;
    }

    public static T PickRandom<T>(this List<T> list)
    {
        if (list.Count == 0)
            return default(T);
        if (list.Count == 1)
            return list[0];
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static void Shuffle<T>(this List<T> list)
    {
        int chosen = -1;
        T temp;
        for (int i = list.Count - 1; i >= 1; i--)
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
