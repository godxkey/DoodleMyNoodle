using System;
using Unity.Collections;
using Unity.Entities;

public enum DesignerFriendlyTeam
{
    Player = 0,
    Baddies = 1
}

[Serializable]
public struct Team : IComponentData, IEquatable<Team>
{
    public int Value;

    public bool Equals(Team other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        return obj is Team t && this == t;
    }

    public override int GetHashCode()
    {
        return -1937169414 + Value.GetHashCode();
    }

    public static bool operator ==(Team left, Team right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Team left, Team right)
    {
        return !(left == right);
    }

    public static implicit operator int(Team val) => val.Value;
    public static implicit operator Team(int val) => new Team() { Value = val };

    public static Team None => new Team() { Value = -1 };
    public static Team Null => new Team() { Value = -2 };
}

public partial class CommonReads
{
    public static NativeList<Entity> GetEntitiesFromTeam(ISimWorldReadAccessor accessor, int teamID)
    {
        NativeList<Entity> entitiesFromTeam = new NativeList<Entity>(Allocator.Temp);
        accessor.Entities.ForEach((Entity entity, ref Team team) => 
        {
            if(team.Value == teamID)
            {
                entitiesFromTeam.Add(entity);
            }   
        });
        return entitiesFromTeam;
    }
}
