using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropper<T>
{
    /// <summary>
    /// 2 = double drop speed
    /// <para/>
    /// 1 = normal drop speed
    /// <para/>
    /// 0 = no more drops
    /// </summary>
    public float speed { get; set; } = 1;
    public float normalDeltaTime { get; set; }
    public int queueLength => queue.Count;

    float lastDropTime = -1;
    Queue<T> queue = new Queue<T>();

    float speedAffectedDeltaTime => normalDeltaTime / speed;
    float expectedNextDropTime => lastDropTime + speedAffectedDeltaTime;

    public Dropper(float normalDeltaTime)
    {
        this.normalDeltaTime = normalDeltaTime;
    }

    public bool IsReadyForDrop(float time)
    {
        float timeElapsedSinceLastDrop = time - lastDropTime;
        return timeElapsedSinceLastDrop >= speedAffectedDeltaTime;
    }

    public void Enqueue(T item) => queue.Enqueue(item);

    public bool TryDrop(float time, out T droppedItem)
    {
        if (IsReadyForDrop(time) && queue.Count > 0)
        {
            droppedItem = queue.Dequeue();


            // this is to account for irregularities
            // For example, if we want 1 drop per second, it's not going to happen like this:
            //          18.00s - 19.00s - 20.00s - 21.00s ...
            // It's going to happen like this:
            //          18.00s - 19.04s - 20.09s - 21.12s ...
            // To make sure those irregularities don't add up, we fake that the drop actually happened on time
            //   (fake) 18.00s - 19.00s - 20.00s - 21.00s ...
            //   (real) 18.03s - 19.01s - 20.04s - 21.02s ...
            lastDropTime = Mathf.Max(expectedNextDropTime, time - speedAffectedDeltaTime);

            return true;
        }
        else
        {
            droppedItem = default;
            return false;
        }
    }
}
