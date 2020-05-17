using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class TimerBarDisplay : SingletonEntityDataDisplay<TurnTimer>
{
    public Slider TimerBar;

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        if(_singletonData != Entity.Null)
        {
            TimerBar.value = (float)(SimWorld.GetComponentData<TurnTimer>(_singletonData).Value / SimWorld.GetComponentData<TurnDuration>(_singletonData).Value);
        }
    }
}
