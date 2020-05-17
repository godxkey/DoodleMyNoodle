using System.Collections.Generic;
using UnityEngineX;

/// <summary>
/// Allows faster .Remove() from list
/// </summary>
public interface IIndexedInList
{
    int Index { get; set; }
}

public static class IndexedInListExtensions
{
    /// <summary>
    /// Add item and adjust index. (Necessary to be able to use RemoveIndexed)
    /// </summary>
    public static void AddIndexed<T>(this List<T> list, T item) where T : IIndexedInList
    {
        item.Index = list.Count;
        list.Add(item);
    }

    /// <summary>
    /// Remove item in O(1)
    /// </summary>
    public static void RemoveIndexed<T>(this List<T> list, T item) where T : IIndexedInList
    {
        int i = item.Index;

        // remove item
        list.RemoveWithLastSwapAt(i);

        // adjust 'index' of the instance we just swapped with
        if (list.Count > i)
        {
            list[i].Index = i;
        }
    }
}