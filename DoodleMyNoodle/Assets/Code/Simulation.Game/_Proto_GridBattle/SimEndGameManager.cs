using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimEndGameManager : SimSingleton<SimEndGameManager>, ISimTickable
{
    public static bool ReturnToMenu = false;

    public bool GameEnded = false;
    public Team WinningTeam;

    void ISimTickable.OnSimTick()
    {
        if (Simulation.Time < 5)
            return;

        List<Team> teamsWithPawns = new List<Team>();
        foreach (SimPawnComponent pawn in Simulation.EntitiesWithComponent<SimPawnComponent>())
        {
            SimTeamMemberComponent teamMemberComponent = pawn.GetComponent<SimTeamMemberComponent>();
            if (teamMemberComponent)
            {
                teamsWithPawns.AddUnique(teamMemberComponent.Team);
            }
        }

        if (!teamsWithPawns.Contains(Team.AI))
        {
            GameOver(Team.Player);
        }
        else if(!teamsWithPawns.Contains(Team.Player))
        {
            GameOver(Team.AI);
        }
    }

    private void GameOver(Team winningTeam)
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
