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
        public TeamAuth.DesignerFriendlyTeam Team;
        public string Text;
    }

    public TextMeshProUGUI TextDisplay;

    public List<TeamTurnText> TeamTurnTexts = new List<TeamTurnText>();

    protected override void OnGamePresentationUpdate()
    {
        int currentTeam = -1;

        if (SimWorld.TryGetSingleton(out TurnCurrentTeam turnCurrentTeam))
        {
            currentTeam = turnCurrentTeam.Value;
        }

        SetCurrentTeam((TeamAuth.DesignerFriendlyTeam)currentTeam);
    }

    private void SetCurrentTeam(TeamAuth.DesignerFriendlyTeam team)
    {
        for (int i = 0; i < TeamTurnTexts.Count; i++)
        {
            if(TeamTurnTexts[i].Team == team)
            {
                TextDisplay.text = TeamTurnTexts[i].Text;
                return;
            }
        }

        TextDisplay.text = "Wait";
    }
}
