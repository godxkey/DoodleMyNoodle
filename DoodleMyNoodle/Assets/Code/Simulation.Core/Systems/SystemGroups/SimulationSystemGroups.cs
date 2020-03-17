using Unity.Entities;

public interface IManualSystemGroupUpdate
{
    bool CanUpdate { get; set; }
    void Update();
}

[UnityEngine.ExecuteAlways]
[AlwaysUpdateSystem]
public class SimPreInitializationSystemGroup : ComponentSystemGroup, IManualSystemGroupUpdate
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
public class SimSimulationSystemGroup : SimulationSystemGroup, IManualSystemGroupUpdate
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
public class SimInitializationSystemGroup : InitializationSystemGroup, IManualSystemGroupUpdate
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
public class SimPresentationSystemGroup : PresentationSystemGroup, IManualSystemGroupUpdate
{
    public bool CanUpdate { get; set; }
    protected override void OnUpdate()
    {
        if (!CanUpdate)
            return;

        base.OnUpdate();
    }
}