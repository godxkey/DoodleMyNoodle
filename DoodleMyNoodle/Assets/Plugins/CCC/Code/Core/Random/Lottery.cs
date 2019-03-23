using System.Collections.Generic;
using UnityEngine;

public interface IWeight
{
    float Weight { get; }
}

public class Lottery<T>
{
    List<IWeight> list;

    private class WrappedObject : IWeight
    {
        public float weight;
        public T obj;
        public float Weight { get { return weight; } set { weight = value; } }
        public WrappedObject(T obj, float weight) { this.weight = weight; this.obj = obj; }
    }

    public Lottery(int capacity = 4)
    {
        TotalWeight = 0;
        list = new List<IWeight>(capacity);
    }
    public Lottery(IEnumerable<IWeight> c)
    {
        TotalWeight = 0;
        foreach (IWeight item in c)
        {
            TotalWeight += item.Weight;
        }
        list = new List<IWeight>(c);
    }

    /// <summary>
    /// Number of elements in the lottery
    /// </summary>
    public int Count
    {
        get
        {
            return list.Count;
        }
    }

    /// <summary>
    /// Combined weight of all the elements
    /// </summary>
    public float TotalWeight { get; private set; }

    /// <summary>
    /// Add an element in the lottery. The element must inherit from IWeight so we can determine its winning chances.
    /// </summary>
    public void Add<U>(U obj) where U : IWeight, T
    {
        list.Add(obj);
        TotalWeight += obj.Weight;
    }

    /// <summary>
    /// Add an element in the lottery and specify its winning chances (weight)
    /// </summary>
    public void Add(T obj, float weight)
    {
        list.Add(new WrappedObject(obj, weight));
        TotalWeight += weight;
    }

    /// <summary>
    /// Add multiple elements in the lottery. The elements must inherit from IWeight so we can determine their winning chances.
    /// </summary>
    public void AddRange(IEnumerable<IWeight> r)
    {
        foreach (var item in r)
        {
            TotalWeight += item.Weight;
        }
        list.AddRange(r);
    }

    /// <summary>
    /// Remove an element from the lottery
    /// </summary>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= list.Count)
            throw new System.Exception("Tried to remove lottery element out of range.");
        TotalWeight -= list[index].Weight;
        list.RemoveAt(index);
    }

    /// <summary>
    /// Remove every element from the lottery
    /// </summary>
    public void Clear()
    {
        list.Clear();
        TotalWeight = 0;
    }

    /// <summary>
    /// Pick a random winner in the lottery.
    /// </summary>
    /// <param name="pickedIndex">The index of the picked element</param>
    /// <returns></returns>
    public T Pick(out int pickedIndex)
    {
        pickedIndex = -1;

        if (list.Count <= 0)
        {
            DebugService.LogError("No lottery item to pick from. Add some before picking.");
            return default(T);
        }

        float ticket = Random.Range(0, TotalWeight);
        float currentWeight = 0;
        for (pickedIndex = 0; pickedIndex < list.Count; pickedIndex++)
        {
            currentWeight += list[pickedIndex].Weight;
            if (ticket < currentWeight)
            {
                if (list[pickedIndex] is WrappedObject)
                    return (list[pickedIndex] as WrappedObject).obj;          //Devrais toujours return ici
                else
                    return (T)list[pickedIndex];
            }
        }

        DebugService.LogError("Error in lottery.");
        return default(T);
    }

    /// <summary>
    /// Pick a random winner in the lottery.
    /// </summary>
    public T Pick()
    {
        int bidon;
        return Pick(out bidon);
    }
}
