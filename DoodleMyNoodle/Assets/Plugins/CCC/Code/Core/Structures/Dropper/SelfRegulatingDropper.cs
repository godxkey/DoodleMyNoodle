using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This dropper class will regulate the drop speed depending on the current queue length.
/// <para/>
/// If queue length is less than or equal to 'expected': speed = 1
/// <para/>
/// If queue length 2x 'expected': speed = maxSpeed
/// </summary>
public class SelfRegulatingDropper<T> : Dropper<T>
{
    public float speedIncreasePerExtraItem { get; set; }
    public int expectedQueueLength { get; set; }

    public SelfRegulatingDropper(float normalDeltaTime, int expectedQueueLength, float speedIncreasePerExtraItem = 1.25f)
        : base(normalDeltaTime)
    {
        this.speedIncreasePerExtraItem = speedIncreasePerExtraItem;
        this.expectedQueueLength = expectedQueueLength;
    }

    public void UpdateSpeed()
    {
        int extraItems = (queueLength - expectedQueueLength).MinLimit(0);

        speed = speed + (extraItems * speedIncreasePerExtraItem);
    }
}
