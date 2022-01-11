using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;

public class TurnTeamDisplay : GamePresentationBehaviour
{
    [System.Serializable]
    public struct TeamTurnText
    {
        public DesignerFriendlyTeam Team;
        public string Text;
    }

    public TextMeshProUGUI TextDisplay;

    public List<TeamTurnText> TeamTurnTexts = new List<TeamTurnText>();

    protected override void OnGamePresentationUpdate()
    {
        int currentTeam = -1;

        foreach (var item in Cache.CurrentlyPlayingControllers)
        {
            if (SimWorld.TryGetComponent(item, out Team team))
            {
                currentTeam = team.Value;
                break;
            }
        }

        SetCurrentTeam((DesignerFriendlyTeam)currentTeam);
    }

    private void SetCurrentTeam(DesignerFriendlyTeam team)
    {
        for (int i = 0; i < TeamTurnTexts.Count; i++)
        {
            if (TeamTurnTexts[i].Team == team)
            {
                TextDisplay.text = TeamTurnTexts[i].Text;
                return;
            }
        }

        TextDisplay.text = "Wait";
    }
}
