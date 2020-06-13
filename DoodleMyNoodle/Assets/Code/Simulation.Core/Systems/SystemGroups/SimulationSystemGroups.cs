using Unity.Entities;

internal interface IManualSystemGroupUpdate
{
    bool CanUpdate { get; set; }
    void Update();
}

[UnityEngine.ExecuteAlways]
[AlwaysUpdateSystem]
internal class SimPreInitializationSystemGroup : ComponentSystemGroup, IManualSystemGroupUpdate
{
    public bool CanUpdate { get; set; }
    protected override void OnUpdate()
    {
        if (!CanUpdate)
            return;

        base.OnUpdate();
    }

    // This gives the presentation access to a query builder inside the sim. Not sure if its ok ...
    public EntityQueryBuilder QueryBuilder => Entities;
}
[UnityEngine.ExecuteAlways]
[AlwaysUpdateSystem]
internal class SimInitializationSystemGroup : InitializationSystemGroup, IManualSystemGroupUpdate
{
    public bool CanUpdate { get; set; }
    protected override void OnUpdate()
    {
        if (!CanUpdate)
            return;

        base.OnUpdate();
    }
}
[UnityEngine.ExecuteAlways]
[AlwaysUpdateSystem]
internal class SimSimulationSystemGroup : SimulationSystemGroup, IManualSystemGroupUpdate
{
    public bool CanUpdate { get; set; }
    protected override void OnUpdate()
    {
        if (!CanUpdate)
            return;

        base.OnUpdate();
    }
}
[UnityEngine.ExecuteAlways]
[AlwaysUpdateSystem]
internal class SimPresentationSystemGroup : PresentationSystemGroup, IManualSystemGroupUpdate
{
    public bool CanUpdate { get; set; }
    protected override void OnUpdate()
    {
        if (!CanUpdate)
            return;

        base.OnUpdate();
    }
}
[UnityEngine.ExecuteAlways]
[AlwaysUpdateSystem]
internal class SimPostPresentationSystemGroup : ComponentSystemGroup, IManualSystemGroupUpdate
{
    public bool CanUpdate { get; set; }
    protected override void OnUpdate()
    {
        if (!CanUpdate)
            return;

        base.OnUpdate();
    }
}