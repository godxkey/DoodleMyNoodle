using SimulationControl;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class MasterOnlyAttribute : Attribute
{

}
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ClientOnlyAttribute : Attribute
{

}

[UpdateAfter(typeof(TickSimulationSystem))]
[UpdateInGroup(typeof(SimulationControlSystemGroup))]
public class ViewSystemGroup : ComponentSystemGroup, IManualSystemGroupUpdate
{
    public bool CanUpdate { get; set; }


    protected override void OnCreate()
    {
        base.OnCreate();

        // TODO: create/destroy systems + Initialize() + Shutdown() pattern

        var simControlGroup = World.GetOrCreateSystem<SimulationControlSystemGroup>();

        IEnumerable<Type> viewComponentSystemTypes =

                    // get all ViewComponent systems
                    TypeUtility.GetECSTypesDerivedFrom(typeof(ViewComponentSystem))
            .Concat(TypeUtility.GetECSTypesDerivedFrom(typeof(ViewJobComponentSystem)))

            // exlude those with the DisableAutoCreate attribute
            .Where((type) =>
            {
                if (Attribute.IsDefined(type, typeof(DisableAutoCreationAttribute), true))
                    return false;

                if (!simControlGroup.IsClient && Attribute.IsDefined(type, typeof(ClientOnlyAttribute), true))
                    return false;

                if (!simControlGroup.IsMaster && Attribute.IsDefined(type, typeof(MasterOnlyAttribute), true))
                    return false;

                return true;
            });


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
