using Unity.Collections;
using UnityEngineX;

public struct FixLottery<T> where T : unmanaged
{
    private struct Wrapper
    {
        public T Item;
        public fix Weight;
    }

    private NativeList<Wrapper> _list;

    public FixLottery(Allocator allocator, int capacity = 4)
    {
        TotalWeight = 0;
        _list = new NativeList<Wrapper>(capacity, allocator);
    }

    public int Length => _list.Length;

    public fix TotalWeight { get; private set; }

    /// <summary>
    /// Add an element in the lottery and specify its winning chances (weight)
    /// </summary>
    public void Add(T obj, fix weight)
    {
        _list.Add(new Wrapper() { Item = obj, Weight = weight });
        TotalWeight += weight;
    }

    /// <summary>
    /// Remove an element from the lottery
    /// </summary>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _list.Length)
            throw new System.ArgumentOutOfRangeException(nameof(index), index, $"Range is {_list.Length}");
        TotalWeight -= _list[index].Weight;
        _list.RemoveAt(index);
    }

    /// <summary>
    /// Remove every element from the lottery
    /// </summary>
    public void Clear()
    {
        _list.Clear();
        TotalWeight = 0;
    }

    /// <summary>
    /// Pick a random winner in the lottery.
    /// </summary>
    /// <param name="pickedIndex">The index of the picked element</param>
    /// <returns></returns>
    public T Pick(ref FixRandom random, out int pickedIndex)
    {
        pickedIndex = -1;

        if (_list.Length <= 0)
        {
            Log.Error("No lottery item to pick from. Add some before picking.");
            return default(T);
        }

        fix ticket = random.NextFix(0, TotalWeight);
        fix currentWeight = 0;
        for (pickedIndex = 0; pickedIndex < _list.Length; pickedIndex++)
        {
            currentWeight += _list[pickedIndex].Weight;
            if (ticket < currentWeight)
            {
                return this[pickedIndex];
            }
        }

        Log.Error("Error in lottery.");
        return default(T);
    }

    /// <summary>
    /// Pick a random winner in the lottery.
    /// </summary>
    public T Pick(ref FixRandom fixRandom)
    {
        return Pick(ref fixRandom, out _);
    }

    public T this[int index]
    {
        get => _list[index].Item;
    }
}