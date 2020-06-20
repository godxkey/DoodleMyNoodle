using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class TimerBarDisplay : GamePresentationBehaviour
{
    public Slider TimerBar;
    public int TimeToStartShowing = 10;

    public GameObject BarContainer;

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        if (SimWorld.TryGetSingleton(out TurnTimer turnTimer) 
            && SimWorld.TryGetSingleton(out TurnDuration turnDuration)
            && SimWorld.TryGetSingleton(out TurnCurrentTeam turnTeam))
        {
            if (turnTimer.Value <= TimeToStartShowing)
            {
                BarContainer.SetActive(true);
                switch (turnTeam.Value)
                {
                    case (int)TurnSystemSetting.Team.AI:
                        TimerBar.fillRect.GetComponent<Image>().color = Color.red;

                        fix totalTimerBarAITime = turnDuration.DurationAI < TimeToStartShowing ? turnDuration.DurationAI : TimeToStartShowing;

                        TimerBar.value = (float)(turnTimer.Value / totalTimerBarAITime);
                        break;
                    case (int)TurnSystemSetting.Team.Players:
                        TimerBar.fillRect.GetComponent<Image>().color = Color.blue;

                        fix totalTimerBarPlayerTime = turnDuration.DurationPlayer < TimeToStartShowing ? turnDuration.DurationPlayer : TimeToStartShowing;

                        TimerBar.value = (float)(turnTimer.Value / totalTimerBarPlayerTime);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                BarContainer.SetActive(false);
            }
        }
    }
}
