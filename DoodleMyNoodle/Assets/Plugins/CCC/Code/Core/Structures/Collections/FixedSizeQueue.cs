using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FixedSizeQueue<T> : IEnumerable
{
    private int head;
    private T[] buffer;

    /// <summary>
    /// The count will never exceed the capacity
    /// </summary>
    public int Count { get; private set; }
    public int Capacity => buffer.Length;
    public T[] UnderlyingArrayBuffer => buffer;

    public FixedSizeQueue(int capacity)
    {
        buffer = new T[capacity];
    }

    /// <summary>
    /// If the queue is full (count == capacity), the oldest item will be popped
    /// </summary>
    public void Enqueue(T value)
    {
        head--;
        if(head < 0)
        {
            head = Capacity - 1;
        }

        buffer[head] = value;
    }

    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    /// <summary>
    /// The last enqueued item is at index 0
    /// </summary>
    public T this[int i]
    {
        get
        {
            if (i >= Count)
                throw new System.ArgumentOutOfRangeException();

            int realIndex = i + head;

            if (realIndex >= buffer.Length) // loop back if out-of-range
                realIndex -= buffer.Length;

            return buffer[realIndex];
        }
    }

}
