using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dropper<T>
{
    protected struct Element
    {
        public T value;
        public float scheduledDrop;
    }

    /// <summary>
    /// 2 = double drop speed
    /// <para/>
    /// 1 = normal drop speed
    /// <para/>
    /// 0 = no more drops
    /// </summary>
    public float speed;
    public int queueLength => queue.Count;

    protected float currentTime { get; private set; } = 0;
    protected Element lastEnqueuedElement { get; private set; } = new Element() { value = default, scheduledDrop = float.MinValue };
    Queue<Element> _queue;

    public virtual void Update(float deltaTime)
    {
        currentTime += deltaTime * speed;
    }

    public bool IsReadyForDrop()
    {
        if (queue.Count == 0)
            return false;

        return currentTime >= queue.Peek().scheduledDrop;
    }

    public virtual void Enqueue(T item, float deltaTime)
    {
        float scheduleBase = lastEnqueuedElement.scheduledDrop;

        lastEnqueuedElement = new Element()
        {
            value = item,
            scheduledDrop = (scheduleBase + deltaTime).MinLimit(currentTime)
        };

        queue.Enqueue(lastEnqueuedElement);

    }

    public bool TryDrop(out T droppedItem)
    {
        if (IsReadyForDrop())
        {
            droppedItem = queue.Dequeue().value;

            return true;
        }
        else
        {
            droppedItem = default;
            return false;
        }
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
