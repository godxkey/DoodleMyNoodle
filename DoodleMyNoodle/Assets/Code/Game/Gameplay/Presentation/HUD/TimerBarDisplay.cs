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

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.TryGetSingleton(out TurnSystemDataRemainingTurnTime remainingTurnTime) 
            && SimWorld.TryGetSingleton(out TurnSystemDataTimerSettings timerSettings))
        {
            if (remainingTurnTime.Value <= TimeToStartShowing)
            {
                BarContainer.SetActive(true);

                TimerBar.fillRect.GetComponent<Image>().color = Color.blue;
                TimerBar.value = (float)(remainingTurnTime.Value / fixMath.min(timerSettings.TurnDuration, TimeToStartShowing));
            }
            else
            {
                BarContainer.SetActive(false);
            }
        }
    }
}
