using SimulationControl;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

[UpdateAfter(typeof(TickSimulationSystem))]
[UpdateInGroup(typeof(SimulationControlSystemGroup))]
public class ViewSystemGroup : ComponentSystemGroup, IManualSystemGroupUpdate
{
    public bool CanUpdate { get; set; }

    protected override void OnCreate()
    {
        base.OnCreate();

        AddSystemToUpdateList(World.CreateSystem<BeginViewSystem>());
        AddSystemToUpdateList(World.CreateSystem<EndViewSystem>());


        IEnumerable<Type> viewComponentSystemTypes =

                    // get all ViewComponent systems
                    TypeUtility.GetECSTypesDerivedFrom(typeof(ViewComponentSystem))
            .Concat(TypeUtility.GetECSTypesDerivedFrom(typeof(ViewJobComponentSystem)))

            // exlude those with the DisableAutoCreate attribute
            .Where((type) => !Attribute.IsDefined(type, typeof(DisableAutoCreationAttribute), true));


        foreach (var systemType in viewComponentSystemTypes)
        {
            AddSystemToUpdateList(World.GetOrCreateSystem(systemType));
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
