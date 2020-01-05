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
    public static void RemoveFirst<T>(this List<T> list)
    {
        list.RemoveAt(0);
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
    public static bool RemoveWithLastSwap<T>(this List<T> list, T value)
    {
        int index = list.IndexOf(value);
        if (index >= 0)
        {
            list.RemoveWithLastSwapAt(index);
            return true;
        }
        else
        {
            return false;
        }
    }
    public static void RemoveWithLastSwapAt<T>(this List<T> list, int index)
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

    public static void SwapWithLast<T>(this List<T> list, int index)
    {
        if (list.Count <= 1)
            return;

        int lastIndex = list.Count - 1;
        T temp = list[lastIndex];

        //Swap chosen element with element at end of list
        list[lastIndex] = list[index];
        list[index] = temp;
    }

    public static void MoveLast<T>(this List<T> list, int index)
    {
        if (list.Count <= 1)
            return;

        T temp = list[index];

        list.RemoveAt(index);
        list.Add(temp);
    }

    public static T First<T>(this List<T> list)
    {
        return list[0];
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

    public static ReadOnlyList<ListType> AsReadOnlyNoAlloc<ListType>(this List<ListType> list)
    {
        return new ReadOnlyList<ListType>(list);
    }
    public static ReadOnlyList<ListType, ReadType> AsReadOnlwyNoAlloc<ListType, ReadType>(this List<ListType> list)
        where ListType : ReadType
    {
        return new ReadOnlyList<ListType, ReadType>(list);
    }

    public static bool Contains<T>(this IEnumerable enumerable)
    {
        foreach (var item in enumerable)
        {
            if (item is T)
                return true;
        }

        return false;
    }

    public static bool AddUnique<T>(this List<T> list, in T value)
    {
        if (!list.Contains(value))
        {
            list.Add(value);
            return true;
        }
        else
        {
            return false;
        }
    }
}
