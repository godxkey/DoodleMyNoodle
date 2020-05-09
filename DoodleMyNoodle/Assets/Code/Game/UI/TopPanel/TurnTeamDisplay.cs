using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TurnTeamDisplay : GameMonoBehaviour
{
    [System.Serializable]
    public struct TeamTurnText
    {
        public TeamAuth.DesignerFriendlyTeam Team;
        public string Text;
    }

    public List<TeamTurnText> TeamTurnTexts = new List<TeamTurnText>();

    public void SetCurrentTeam()
    {

    }
}
