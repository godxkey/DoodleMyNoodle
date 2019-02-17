using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameService : MonoCoreService<FrameService>
{
    public override void Initialize(Action<ICoreService> onComplete) => onComplete(this);

    public static int FrameCount { get; private set; } = 0;

    void Awake()
    {
        FrameCount = Time.frameCount;

        StartCoroutine(FrameUpdateRoutine());
    }

    IEnumerator FrameUpdateRoutine()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            FrameCount++;
        }
    }

}
