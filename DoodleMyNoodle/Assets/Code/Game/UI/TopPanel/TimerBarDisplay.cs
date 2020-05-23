using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class TimerBarDisplay : GameMonoBehaviour
{
    public Slider TimerBar;

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        if (SimWorld.TryGetSingleton(out TurnTimer turnTimer) && SimWorld.TryGetSingleton(out TurnDuration turnDuration))
        {
            TimerBar.value = (float)(turnTimer.Value / turnDuration.Value);
        }
    }
}
