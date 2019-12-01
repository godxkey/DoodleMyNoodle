using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Player,
    AI
}

public class SimTeams
{
    public const int TEAM_COUNT = 2;
}

public class SimTeamMember : SimComponent
{
    public Team Team;
}
