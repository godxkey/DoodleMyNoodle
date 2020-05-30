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

        if (SimWorld.TryGetSingleton(out TurnTimer turnTimer) 
            && SimWorld.TryGetSingleton(out TurnDuration turnDuration)
            && SimWorld.TryGetSingleton(out TurnCurrentTeam turnTeam))
        {
            TimerBar.value = (float)(turnTimer.Value / turnDuration.Value);

            switch (turnTeam.Value)
            {
                case (int)TurnSystemSetting.Team.AI:
                    TimerBar.fillRect.GetComponent<Image>().color = Color.red;
                    break;
                case (int)TurnSystemSetting.Team.Players:
                    TimerBar.fillRect.GetComponent<Image>().color = Color.blue;
                    break;
                default:
                    break;
            }
        }
    }
}
