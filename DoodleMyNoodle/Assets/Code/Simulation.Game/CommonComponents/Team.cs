using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct Team : IComponentData
{
    public int Value;
}

public partial class CommonReads
{
    public static NativeList<Entity> GetAllEntityFromTeam(ISimWorldReadAccessor accessor, int teamID)
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
