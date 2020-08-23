using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using TMPro;
using System.Collections.Generic;

public class GameOverWinnerDisplay : GamePresentationBehaviour
{
    [System.Serializable]
    public struct WinningTeamText
    {
        public DesignerFriendlyTeam Team;
        public string Text;
    }

    public GameObject DisplayContainer;

    public TextMeshProUGUI TextDisplay;

    public List<WinningTeamText> WinTeamTexts = new List<WinningTeamText>();

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.TryGetSingleton(out WinningTeam currentWinner))
        {
            DisplayContainer.SetActive(true);
            SetCurrentWinningTeam((DesignerFriendlyTeam)currentWinner.Value);

            this.DelayedCall(5, () =>
            {
                GameStateManager.TransitionToState(QuickStartAssets.instance.rootMenu);
            });
        }
    }

    private void SetCurrentWinningTeam(DesignerFriendlyTeam team)
    {
        for (int i = 0; i < WinTeamTexts.Count; i++)
        {
            if (WinTeamTexts[i].Team == team)
            {
                TextDisplay.text = WinTeamTexts[i].Text;
                return;
            }
        }

        TextDisplay.text = "Unknown Team Win";
    }
}
