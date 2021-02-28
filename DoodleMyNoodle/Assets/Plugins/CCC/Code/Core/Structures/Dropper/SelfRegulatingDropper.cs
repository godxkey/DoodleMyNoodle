using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Mathematics.math;
using static Unity.MathematicsX.mathX;

/// <summary>
/// This dropper class will regulate the drop speed depending on the current queue length.
/// <para/>
/// If queue length longer than 'expected': speed = catchUpSpeed
/// </summary>
[System.Serializable]
public class SelfRegulatingDropper<T> : Dropper<T>
{
    public float MaximalCatchUpSpeed = 4;
    public float MaximalExpectedTimeInQueue = 2;

    float _timeLimitForLastItem;

    public SelfRegulatingDropper(float maximalExpectedTimeInQueue, float maximalCatchUpSpeed = 4)
    {
        this.MaximalCatchUpSpeed = maximalCatchUpSpeed;
        this.MaximalExpectedTimeInQueue = maximalExpectedTimeInQueue;
    }

    public override void Update(float deltaTime)
    {
        if (queue.Count > 0)
        {
            float timeUntilQueueEmpty = LastEnqueuedElement.ScheduledDrop - CurrentTime;
            Speed = clamp(timeUntilQueueEmpty / _timeLimitForLastItem, 1, MaximalCatchUpSpeed);
        }
        else
        {
            Speed = 1;
        }


        _timeLimitForLastItem -= deltaTime;
        _timeLimitForLastItem = max(_timeLimitForLastItem, 0.0001f);
        base.Update(deltaTime);
    }

    public override void Enqueue(T item, float deltaTime)
    {
        base.Enqueue(item, deltaTime);
        _timeLimitForLastItem = MaximalExpectedTimeInQueue;
    }
}
