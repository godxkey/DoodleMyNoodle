using Unity.Entities;

[System.Flags]
public enum ActorFilter : byte
{
    None = 0,
    Allies = 1 << 0,
    Enemies = 1 << 1,
    Terrain = 1 << 2
}

public struct ActorFilterInfo
{
    public Team Team;
    public bool IsTerrain;
}

public partial struct Helpers
{
    public static bool ActorFilterMatches(ActorFilterInfo actorA, ActorFilterInfo actorB, ActorFilter filter)
    {
        return ((filter & ActorFilter.Allies) != 0 && actorA.Team == actorB.Team)
            || ((filter & ActorFilter.Enemies) != 0 && actorA.Team != actorB.Team)
            || ((filter & ActorFilter.Terrain) != 0 && actorB.IsTerrain);
    }

    public static ActorFilterInfo GetActorFilterInfo(
        Entity entity,
        ComponentDataFromEntity<Team> Teams,
        ComponentDataFromEntity<FirstInstigator> FirstInstigators,
        ComponentDataFromEntity<TileColliderTag> TileColliderTags)
    {
        ActorFilterInfo result = new ActorFilterInfo()
        {
            Team = Team.None,
            IsTerrain = false,
        };

        // try to get the team
        if (Teams.TryGetComponent(entity, out Team team))
        {
            result.Team = team;
        }
        else if (FirstInstigators.TryGetComponent(entity, out FirstInstigator firstInstigator))
        {
            if (Teams.TryGetComponent(firstInstigator, out Team firstInstigatorTeam))
            {
                result.Team = firstInstigatorTeam;
            }
        }

        result.IsTerrain = TileColliderTags.HasComponent(entity);

        return result;
    }
}

public partial class CommonReads
{
    public static ActorFilterInfo GetActorFilterInfo(ISimGameWorldReadAccessor accessor, Entity entity)
    {
        ActorFilterInfo result = new ActorFilterInfo()
        {
            Team = Team.None,
            IsTerrain = false,
        };

        // try to get the team
        if (accessor.TryGetComponent(entity, out Team team))
        {
            result.Team = team;
        }
        else if (accessor.TryGetComponent(entity, out FirstInstigator firstInstigator))
        {
            if (accessor.TryGetComponent(firstInstigator, out Team firstInstigatorTeam))
            {
                result.Team = firstInstigatorTeam;
            }
        }

        result.IsTerrain = accessor.HasComponent<TileColliderTag>(entity);

        return result;
    }
}