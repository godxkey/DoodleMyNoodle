using CCC.Fix2D;
using Unity.Collections;
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
        return ((filter & ActorFilter.Allies) != 0 && actorA.Team == actorB.Team && actorB.Team != -1)
            || ((filter & ActorFilter.Enemies) != 0 && actorA.Team != actorB.Team && actorB.Team != -1)
            || ((filter & ActorFilter.Terrain) != 0 && actorB.IsTerrain);
    }

    public static ActorFilterInfo GetActorFilterInfo(
        Entity entity,
        ComponentDataFromEntity<Team> Teams,
        ComponentDataFromEntity<FirstInstigator> FirstInstigators,
        ComponentDataFromEntity<PhysicsColliderBlob> Colliders,
        ComponentDataFromEntity<Owner> Owners)
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
            Entity firstInstigatorActor = firstInstigator;
            if (Owners.TryGetComponent(firstInstigatorActor, out Owner owner))
                firstInstigatorActor = owner.Value;
            if (Teams.TryGetComponent(firstInstigatorActor, out Team firstInstigatorTeam))
            {
                result.Team = firstInstigatorTeam;
            }
        }


        result.IsTerrain = Colliders.TryGetComponent(entity, out var colliderRef)
            && colliderRef.Collider.IsCreated 
            && Helpers.BelongsToTerrain(colliderRef.Collider.Value.Filter);

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
            Entity firstInstigatorActor = firstInstigator;
            if (accessor.TryGetComponent(firstInstigatorActor, out Owner owner))
                firstInstigatorActor = owner.Value;

            if (accessor.TryGetComponent(firstInstigatorActor, out Team firstInstigatorTeam))
            {
                result.Team = firstInstigatorTeam;
            }
        }

        result.IsTerrain = accessor.HasComponent<TileColliderTag>(entity);

        return result;
    }

    public static void FilterActors(ISimGameWorldReadAccessor accessor, NativeList<Entity> targets, ActorFilterInfo instigatorFilterInfo, ActorFilter filter)
    {
        for (int i = targets.Length - 1; i >= 0; i--)
        {
            var filterInfo = CommonReads.GetActorFilterInfo(accessor, targets[i]);
            if (!Helpers.ActorFilterMatches(instigatorFilterInfo, filterInfo, filter))
            {
                targets.RemoveAt(i);
            }
        }
    }
}