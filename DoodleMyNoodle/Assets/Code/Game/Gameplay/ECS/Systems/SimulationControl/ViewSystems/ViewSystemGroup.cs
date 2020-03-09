using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

[UpdateAfter(typeof(SimulationWorldUpdaterSystem))]
public class ViewSystemGroup : ComponentSystemGroup, IManualSystemGroupUpdate
{
    public bool CanUpdate { get; set; }

    protected override void OnCreate()
    {
        base.OnCreate();

        m_systemsToUpdate.Add(World.CreateSystem<BeginViewSystem>());
        m_systemsToUpdate.Add(World.CreateSystem<EndViewSystem>());


        IEnumerable<Type> viewComponentSystemTypes =
                    TypeUtility.GetECSTypesDerivedFrom(typeof(ViewComponentSystem))
            .Concat(TypeUtility.GetECSTypesDerivedFrom(typeof(ViewJobComponentSystem)));

        foreach (var systemType in viewComponentSystemTypes)
        {
            AddSystemToUpdateList(World.CreateSystem(systemType));
        }

        SortSystemUpdateList();
    }

    public override void SortSystemUpdateList()
    {
        base.SortSystemUpdateList();

        m_systemsToUpdate.MoveFirst(m_systemsToUpdate.IndexOf(World.GetExistingSystem<BeginViewSystem>()));
        m_systemsToUpdate.MoveLast(m_systemsToUpdate.IndexOf(World.GetExistingSystem<EndViewSystem>()));
    }

    protected override void OnUpdate()
    {
        if (CanUpdate)
            base.OnUpdate();
    }
}