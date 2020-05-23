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

        if(s_singletonData != Entity.Null)
        {
            TimerBar.value = (float)(SimWorld.GetComponentData<TurnTimer>(s_singletonData).Value / SimWorld.GetComponentData<TurnDuration>(s_singletonData).Value);
        }
    }
}
