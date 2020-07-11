using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct Team : IComponentData, IEquatable<Team>
{
    public int Value;

    public bool Equals(Team other)
    {
        return Value == other.Value;
    }

    public static bool operator ==(Team left, Team right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Team left, Team right)
    {
        return !(left == right);
    }
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
