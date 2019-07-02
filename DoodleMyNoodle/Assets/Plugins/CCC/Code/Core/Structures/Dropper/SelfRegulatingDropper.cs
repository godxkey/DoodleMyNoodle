using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This dropper class will regulate the drop speed depending on the current queue length.
/// <para/>
/// If queue length longer than 'expected': speed = catchUpSpeed
/// </summary>
[System.Serializable]
public class SelfRegulatingDropper<T> : Dropper<T>
{
    public float maximalCatchUpSpeed = 4;
    public float maximalExpectedTimeInQueue = 2;

    float timeLimitForLastItem;

    public SelfRegulatingDropper(float maximalExpectedTimeInQueue, float maximalCatchUpSpeed = 4)
    {
        this.maximalCatchUpSpeed = maximalCatchUpSpeed;
        this.maximalExpectedTimeInQueue = maximalExpectedTimeInQueue;
    }

    public override void Update(float deltaTime)
    {
        if (queue.Count > 0)
        {
            float timeUntilQueueEmpty = lastEnqueuedElement.scheduledDrop - currentTime;
            speed = (timeUntilQueueEmpty / timeLimitForLastItem).Clamped(1, maximalCatchUpSpeed);
        }
        else
        {
            speed = 1;
        }


        timeLimitForLastItem -= deltaTime;
        timeLimitForLastItem = timeLimitForLastItem.MinLimit(0.0001f);
        base.Update(deltaTime);
    }

    public override void Enqueue(T item, float deltaTime)
    {
        base.Enqueue(item, deltaTime);
        timeLimitForLastItem = maximalExpectedTimeInQueue;
    }
}
