using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class SimEndGameManager : SimSingleton<SimEndGameManager>, ISimTickable
{
    public static bool ReturnToMenu = false;

    public bool GameEnded = false;
    public OLD_Team WinningTeam;

    void ISimTickable.OnSimTick()
    {
        if (Simulation.Time < 5)
            return;

        List<OLD_Team> teamsWithPawns = new List<OLD_Team>();
        foreach (SimPawnComponent pawn in Simulation.EntitiesWithComponent<SimPawnComponent>())
        {
            SimTeamMemberComponent teamMemberComponent = pawn.GetComponent<SimTeamMemberComponent>();
            if (teamMemberComponent)
            {
                teamsWithPawns.AddUnique(teamMemberComponent.Team);
            }
        }

        if (!teamsWithPawns.Contains(OLD_Team.AI))
        {
            GameOver(OLD_Team.Player);
        }
        else if(!teamsWithPawns.Contains(OLD_Team.Player))
        {
            GameOver(OLD_Team.AI);
        }
    }

    private void GameOver(OLD_Team winningTeam)
    {
        if (!GameEnded)
        {
            WinningTeam = winningTeam;
            GameEnded = true;

            this.DelayedCall(3, () => 
            {
                ReturnToMenu = true;
            });
        }
    }
}
