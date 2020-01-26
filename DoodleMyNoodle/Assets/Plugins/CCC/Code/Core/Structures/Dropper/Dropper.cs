using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dropper<T>
{
    protected struct Element
    {
        public T Value;
        public float ScheduledDrop;
    }

    /// <summary>
    /// 2 = double drop speed
    /// <para/>
    /// 1 = normal drop speed
    /// <para/>
    /// 0 = no more drops
    /// </summary>
    public float Speed;
    public int QueueLength => queue.Count;

    protected float CurrentTime { get; private set; } = 0;
    protected Element LastEnqueuedElement { get; private set; } = new Element() { Value = default, ScheduledDrop = float.MinValue };
    Queue<Element> _queue;

    public virtual void Update(float deltaTime)
    {
        CurrentTime += deltaTime * Speed;
    }

    public bool IsReadyForDrop()
    {
        if (queue.Count == 0)
            return false;

        return CurrentTime >= queue.Peek().ScheduledDrop;
    }

    public virtual void Enqueue(T item, float deltaTime)
    {
        float scheduleBase = LastEnqueuedElement.ScheduledDrop;

        LastEnqueuedElement = new Element()
        {
            Value = item,
            ScheduledDrop = (scheduleBase + deltaTime).MinLimit(CurrentTime)
        };

        queue.Enqueue(LastEnqueuedElement);

    }

    public bool TryDrop(out T droppedItem)
    {
        if (IsReadyForDrop())
        {
            droppedItem = queue.Dequeue().Value;

            return true;
        }
        else
        {
            droppedItem = default;
            return false;
        }
    }

    public void Clear()
    {
        _queue.Clear();
    }


    // this is necessary to avoid null reference exceptions in unity because it can't serialize a queue it's always null by default
    protected Queue<Element> queue
    {
        get
        {
            if (_queue == null)
                _queue = new Queue<Element>();
            return _queue;
        }
    }

}
